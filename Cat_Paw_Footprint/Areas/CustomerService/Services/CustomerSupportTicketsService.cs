using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Areas.CustomerService.ViewModel;
using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.CustomerService.Services
{
	/// <summary>
	/// 客戶服務工單服務層，負責工單的商業邏輯與 ViewModel 轉換
	/// </summary>
	public class CustomerSupportTicketsService : ICustomerSupportTicketsService
	{
		private readonly ICustomerSupportTicketsRepository _repo;

		/// <summary>
		/// 透過 DI 注入工單 Repository
		/// </summary>
		public CustomerSupportTicketsService(ICustomerSupportTicketsRepository repo)
		{
			_repo = repo;
		}

		/// <summary>
		/// 取得所有工單（轉成 ViewModel 回傳）
		/// </summary>
		public async Task<IEnumerable<CustomerSupportTicketViewModel>> GetAllAsync()
		{
			var tickets = await _repo.GetAllAsync();
			return tickets.Select(t => new CustomerSupportTicketViewModel
			{
				TicketID = t.TicketID,
				CustomerID = t.CustomerID,
				EmployeeID = t.EmployeeID,
				Subject = t.Subject,
				TicketTypeID = t.TicketTypeID,
				Description = t.Description,
				StatusID = t.StatusID,
				PriorityID = t.PriorityID,
				CreateTime = t.CreateTime,
				UpdateTime = t.UpdateTime,
				TicketCode = t.TicketCode,
				Customer = t.Customer,
				Employee = t.Employee,
				Priority = t.Priority,
				Status = t.Status,
				TicketType = t.TicketType
			});
		}

		/// <summary>
		/// 依工單 ID 取得單筆工單（ViewModel）
		/// </summary>
		public async Task<CustomerSupportTicketViewModel?> GetByIdAsync(int id)
		{
			var t = await _repo.GetByIdAsync(id);
			if (t == null) return null;
			return new CustomerSupportTicketViewModel
			{
				TicketID = t.TicketID,
				CustomerID = t.CustomerID,
				EmployeeID = t.EmployeeID,
				Subject = t.Subject,
				TicketTypeID = t.TicketTypeID,
				Description = t.Description,
				StatusID = t.StatusID,
				PriorityID = t.PriorityID,
				CreateTime = t.CreateTime,
				UpdateTime = t.UpdateTime,
				TicketCode = t.TicketCode,
				Customer = t.Customer,
				Employee = t.Employee,
				Priority = t.Priority,
				Status = t.Status,
				TicketType = t.TicketType
			};
		}

		/// <summary>
		/// 新增工單（將 ViewModel 轉成 Entity 並存進資料庫）
		/// </summary>
		public async Task AddAsync(CustomerSupportTicketViewModel vm)
		{
			// 加入除錯訊息，確認資料傳遞狀況
			Console.WriteLine($"[AddAsync] vm.EmployeeID={vm.EmployeeID}, vm.TicketCode={vm.TicketCode}");
			var entity = new CustomerSupportTickets
			{
				CustomerID = vm.CustomerID,
				EmployeeID = vm.EmployeeID, // ★
				Subject = vm.Subject,
				TicketTypeID = vm.TicketTypeID,
				Description = vm.Description,
				StatusID = vm.StatusID,
				PriorityID = vm.PriorityID,
				CreateTime = vm.CreateTime ?? DateTime.Now,
				UpdateTime = vm.UpdateTime ?? DateTime.Now,
				TicketCode = vm.TicketCode // ★
			};
			Console.WriteLine($"[AddAsync] entity.EmployeeID={entity.EmployeeID}, entity.TicketCode={entity.TicketCode}");
			await _repo.AddAsync(entity);
		}

		/// <summary>
		/// 更新工單（先取出原本資料再修改）
		/// </summary>
		public async Task UpdateAsync(CustomerSupportTicketViewModel vm)
		{
			var entity = await _repo.GetByIdAsync(vm.TicketID);
			if (entity == null) return;
			entity.CustomerID = vm.CustomerID;
			entity.EmployeeID = vm.EmployeeID;
			entity.Subject = vm.Subject;
			entity.TicketTypeID = vm.TicketTypeID;
			entity.Description = vm.Description;
			entity.StatusID = vm.StatusID;
			entity.PriorityID = vm.PriorityID;
			entity.UpdateTime = DateTime.Now;
			entity.TicketCode = vm.TicketCode;

			await _repo.UpdateAsync(entity);
		}

		/// <summary>
		/// 刪除工單（依 ID）
		/// </summary>
		public async Task DeleteAsync(int id)
		{
			await _repo.DeleteAsync(id);
		}

		/// <summary>
		/// 檢查指定工單是否存在
		/// </summary>
		public async Task<bool> ExistsAsync(int id)
		{
			return await _repo.ExistsAsync(id);
		}
	}
}