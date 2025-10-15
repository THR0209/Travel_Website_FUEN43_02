using Cat_Paw_Footprint.Areas.CustomersArea.Repositories;
using Cat_Paw_Footprint.Areas.CustomersArea.ViewModel;
using Cat_Paw_Footprint.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace Cat_Paw_Footprint.Areas.CustomersArea.Services
{
	public class CusLogRegService: ICusLogRegService//登入註冊修改資料
	{
		private readonly UserManager<IdentityUser> _userManager;// 用 Identity 管帳號
		private readonly RoleManager<IdentityRole> _roleManager;// 如果要加角色
		private readonly ApplicationDbContext _context;// EF Core
		private readonly ICusLogRegRepository _repo;// 注入 Repository
		private readonly IEmailSender _emailSender;


		public CusLogRegService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ICusLogRegRepository repo, IEmailSender emailSender)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
			_repo = repo;
			_emailSender = emailSender;

		}

		public async Task<CusLogRegDto?> LoginAsync(string account, string password, string ip)// 客戶登入
		{
			var customer = await _repo.GetCustomerByAccountAsync(account);// 根據帳號查詢客戶
			if (customer == null)
			{
				// 帳號不存在或被停用
				return new CusLogRegDto
				{
					ErrorMessage = "帳號不存在"
				};
			}
			if (customer.IsBlacklisted == true)
			{
				// 帳號黑名單
				return new CusLogRegDto
				{
					ErrorMessage = "帳號已被黑名單，請聯絡管理員"
				};
			}
			var user = await _userManager.FindByNameAsync(account);
			if (user == null)
			{
				return new CusLogRegDto
				{
					ErrorMessage = "帳號已遺失，請聯絡管理員"
				};
			}
			var result = await _userManager.CheckPasswordAsync(user, password);
			if (!result)
			{
				await _userManager.AccessFailedAsync(user);

				// 檢查是否已達鎖定條件
				if (await _userManager.IsLockedOutAsync(user))
				{
					return new CusLogRegDto
					{
						ErrorMessage = "帳號已鎖定，請 5 分鐘後再試。"
					};
				}
				// 失敗：寫入紀錄
				await _repo.CreateLoginLogAsync(customer.CustomerId, false, ip);

				// 最近三次都錯誤 → 發信通知
				var loginLogs = await _repo.GetLastLoginHistoryAsync(customer.CustomerId);
				int x =loginLogs.Count();
				if (loginLogs.Count() >= 3 && loginLogs.Take(3).All(log => log.IsSuccessful == false))
				{
					var ipList = string.Join("<br/>",
						loginLogs.Select(log => $"{log.LoginTime:yyyy-MM-dd HH:mm:ss} - {log.LoginIP}"));

					var subject = "【系統提醒】您的帳號連續 3 次登入失敗";
					var body = $@"
                <p>您好 {customer.FullName},</p>
                <p>您的帳號最近有 3 次登入失敗紀錄，登入來源如下：</p>
                <p>{ipList}</p>
                <p>如果不是您本人操作，建議您立即更改密碼，或聯絡客服確認帳號安全。</p>";

					await _emailSender.SendEmailAsync(customer.Email, subject, body);
				}
				
				return new CusLogRegDto { ErrorMessage = "密碼錯誤" };
			}

			await _userManager.ResetAccessFailedCountAsync(user);
			// ✅ 成功：寫入成功紀錄並回傳資料
			await _repo.CreateLoginLogAsync(customer.CustomerId, true, ip);

			return new CusLogRegDto
			{
				CustomerId = customer.CustomerId,
				UserId = customer.UserId,
				Account = customer.Account,
				FullName = customer.FullName,
				Email = customer.Email,
				Phone = customer.Phone,
				Address = customer.Address,
				Level = customer.Level,
				LevelName = customer.LevelName,
				ErrorMessage = string.Empty,
				Message = "登入成功"
			};
		}

		public async Task<string?> RegisterCustomerAsync(CusLogRegDto model)// 客戶註冊
		{
			using var transaction = await _context.Database.BeginTransactionAsync();//開始交易
			try
			{
				if (await _context.Customers.AnyAsync(c => c.Account == model.Account))
					return "帳號已存在";
				if (await _context.CustomerProfiles.AnyAsync(cp => cp.Email == model.Email))
					return "Email 已被使用";
				
				var user = new IdentityUser
				{
					UserName = model.Account,
					Email = model.Email
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
				{
					
					var errors = string.Join(";", result.Errors.Select(e => e.Description));
					return $"建立帳號失敗: {errors}";
				}
				// 確保角色存在
				var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
				if (!roleResult.Succeeded)
				{
					
					return $"角色新增失敗: {string.Join(";", roleResult.Errors.Select(e => e.Description))}";
				}
				var customer = new Models.Customers//建立客戶物件
				{
					UserId = user.Id,
					Account = model.Account,
					Level = 0,//沒付錢
					CreateDate = DateTime.Now,
					Status = true,
					FullName = model.FullName,
					

				};
				_context.Customers.Add(customer);
				await _context.SaveChangesAsync(); // 這裡 EF 會幫你填好 CustomerId

				// 再利用 CustomerId 產生 CustomerCode
				customer.CustomerCode = $"cus{customer.CustomerID:D4}";
				

				var customerProfile = new Models.CustomerProfile//建立客戶詳細資料物件
				{
					Customer = customer,
					CustomerName = model.FullName,
					Email = model.Email,
					Phone = model.Phone,
					Address = model.Address,
					IDNumber = model.IDNumber
				};
				
				_context.CustomerProfiles.Add(customerProfile);
				await _context.SaveChangesAsync();
				
				await transaction.CommitAsync();//成功後交易結束
				return "註冊成功";
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();//失敗後交易回復
				var inner = ex.InnerException?.Message ?? "";
				return $"例外錯誤: {ex.Message}{inner}";
			}
		}

		public async Task<CusLogRegDto?> UpdateCustomerAsync(CusLogRegDto model)// 客戶修改資料
		{
			using var transaction = await _context.Database.BeginTransactionAsync();//開始交易
			try
			{
				
					var customer = await _context.Customers
						.Include(c => c.CustomerProfile)
						.FirstOrDefaultAsync(c => c.CustomerID == model.CustomerId);
					if (customer == null)
					{
						return new CusLogRegDto
						{
							ErrorMessage = "資料錯誤，請聯絡客服人員"
						};
					}
					customer.FullName = model.FullName;
					customer.CustomerProfile.CustomerName = model.FullName;
					customer.CustomerProfile.Email = model.Email;
					customer.CustomerProfile.Phone = model.Phone;
					customer.CustomerProfile.Address = model.Address;
					customer.CustomerProfile.IDNumber = model.IDNumber;
				
				//_context.Customers.Update(customer); // 不需要這行，EF Core 會自動追蹤變更
				//以下更新identity user的email跟電話
				var user = await _userManager.FindByIdAsync(customer.UserId);
					if (user != null)
					{
						user.Email = model.Email;
						user.PhoneNumber = model.Phone;
						var result = await _userManager.UpdateAsync(user);
						if (!result.Succeeded)
						{
						throw new Exception(string.Join(";", result.Errors.Select(e => e.Description)));
						}
					}
				await _context.SaveChangesAsync();
				// 儲存變更


				await transaction.CommitAsync();
				return new CusLogRegDto
				{
					Message = "資料更新成功",
				};
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return new CusLogRegDto
				{
					ErrorMessage = $"例外錯誤: {ex.Message}"
				};
			}
		}

		public async Task<CusLogRegDto?> GetCustomerByAccountAsync(string Account)// 根據帳號查詢客戶
		{
			return await _repo.GetCustomerByAccountAsync(Account);
		}
	}
}
