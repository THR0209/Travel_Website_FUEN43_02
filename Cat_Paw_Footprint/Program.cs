using Cat_Paw_Footprint.Areas.CustomersArea.Repositories;
using Cat_Paw_Footprint.Areas.CustomersArea.Services;
using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.Services;
using Cat_Paw_Footprint.Areas.Order.Models;
using Cat_Paw_Footprint.Areas.Order.Services;
using Cat_Paw_Footprint.Areas.TourGuideArea.Repositories;
using Cat_Paw_Footprint.Areas.TourGuideArea.Services;
using Cat_Paw_Footprint.Areas.Vendor.Repositories;
using Cat_Paw_Footprint.Areas.Vendor.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Hubs;
using Cat_Paw_Footprint.Models;
using Cat_Paw_Footprint.Repositories;
using Cat_Paw_Footprint.Services;
using ClosedXML.Parser;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Cat_Paw_Footprint.Areas.CustomersArea.Controllers.PaymentController;

namespace Cat_Paw_Footprint
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// 1️⃣ 取得 Google Cloud SQL 連線字串
			var credential = GoogleCredential.FromFile(@"C:\GoogleCloudSql\Keys\web-travel-ap.json");
			var client = new SecretManagerServiceClientBuilder { Credential = credential }.Build();
			var secretVersionName = new SecretVersionName("web-travel-473102", "sqlserver-connection", "latest");
			var result = client.AccessSecretVersion(secretVersionName);
			string connectionStringGoogleDB = result.Payload.Data.ToStringUtf8();
			Console.WriteLine($"✅ 從 Secret Manager 取得連線字串: {connectionStringGoogleDB}");

			// 註冊 DbContext (EF Core)
			builder.Services.AddDbContext<webtravel2Context>(options => options.UseSqlServer(connectionStringGoogleDB));
			builder.Services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(connectionStringGoogleDB));
			builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionStringGoogleDB));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			// Identity 註冊
			builder.Services
				.AddIdentity<IdentityUser, IdentityRole>(opt =>
				{
					opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
					opt.Lockout.MaxFailedAccessAttempts = 5;
					opt.Lockout.AllowedForNewUsers = true;
					opt.SignIn.RequireConfirmedAccount = false;
					opt.Password.RequiredLength = 6;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddDefaultUI();

			// 多身分驗證（Vendor/Customer/Employee）
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = "VendorAuth";
				options.DefaultChallengeScheme = "VendorAuth";
			})
			.AddCookie("VendorAuth", options =>
			{
				options.Cookie.Name = ".CatPaw.Vendor.Auth";
				options.LoginPath = "/Vendor/VendorHome/Login";
				options.AccessDeniedPath = "/Vendor/VendorHome/Denied";
			})
			.AddCookie("CustomerAuth", options =>
			{
				options.Cookie.Name = ".CatPaw.Customer.Auth";
				options.LoginPath = "/CustomersArea/Account/Login";
				options.AccessDeniedPath = "/CustomersArea/Account/Index";
			})
			.AddCookie("EmployeeAuth", options =>
			{
				options.Cookie.Name = ".CatPaw.Employee.Auth";
				options.LoginPath = "/Employee/EmployeeAuth/Login";
				options.AccessDeniedPath = "/Home/Index";
			});

			// 授權權限設定
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("Emp.AdminOnly", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "Admin", "SuperAdmin"));

				options.AddPolicy("AreaAdmin", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "Admin", "SuperAdmin"));

				options.AddPolicy("AreaCouponManagement", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "ProductPlanner", "SuperAdmin", "Sales"));

				options.AddPolicy("AreaCustomerService", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "CustomerService", "SuperAdmin"));

				options.AddPolicy("AreaOrder", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "Sales", "SuperAdmin"));

				options.AddPolicy("AreaProductManagement", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "ProductPlanner", "SuperAdmin", "TourGuide"));

				options.AddPolicy("AreaTravelManagement", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "TourGuide", "SuperAdmin", "ProductPlanner"));
			});
			builder.Services.AddDistributedMemoryCache();

			// 🌟 統一 Session 設定（只呼叫一次 AddSession，避免多次呼叫造成 Session Cookie 混亂）
			builder.Services.AddSession(options =>
			{
				options.Cookie.Name = ".CatPaw.Unified.Session";
				options.IdleTimeout = TimeSpan.FromHours(9);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			#region DI 註冊資料存取層與服務層
			builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
			builder.Services.AddScoped<IEmployeeService, EmployeeService>();
			builder.Services.AddScoped<ICustomerAdminRepository, CustomerAdminRepository>();
			builder.Services.AddScoped<ICustomerAdminService, CustomerAdminService>();
			builder.Services.AddScoped<IVendorAdminRepository, VendorAdminRepository>();
			builder.Services.AddScoped<IVendorAdminService, VendorAdminService>();
			builder.Services.AddScoped<IVendorHomeRepository, VendorHomeRepository>();
			builder.Services.AddScoped<IVendorHomeService, VendorHomeService>();
			builder.Services.AddScoped<IFAQService, FAQService>();
			builder.Services.AddScoped<IFAQRepository, FAQRepository>();
			builder.Services.AddScoped<ICustomerSupportTicketsRepository, CustomerSupportTicketsRepository>();
			builder.Services.AddScoped<ICustomerSupportTicketsService, CustomerSupportTicketsService>();
			builder.Services.AddScoped<ICustomerSupportFeedbackService, CustomerSupportFeedbackService>();
			builder.Services.AddScoped<ICustomerSupportFeedbackRepository, CustomerSupportFeedbackRepository>();
			builder.Services.AddScoped<ICustomerSupportMessagesRepository, CustomerSupportMessagesRepository>();
			builder.Services.AddScoped<ICustomerSupportMessagesService, CustomerSupportMessagesService>();
			builder.Services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
			builder.Services.AddScoped<IEmployeeMiniRepository, EmployeeMiniRepository>();
			builder.Services.AddScoped<ICusLogRegRepository, CusLogRegRepository>();
			builder.Services.AddScoped<ICusLogRegService, CusLogRegService>();
			builder.Services.AddScoped<ITGAllRepository, TGAllRepository>();
			builder.Services.AddScoped<ITGAllService, TGAllService>();
			builder.Services.AddSignalR();
			builder.Services.AddScoped<ITalkMessageRepository, TalkMessageRepository>();
			builder.Services.AddScoped<ITalkMessageService, TalkMessageService>();

			#endregion

			builder.Services.AddHttpContextAccessor();

			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();
			builder.Services.AddSignalR();

			builder.Services.Configure<ECPayOptions>(builder.Configuration.GetSection("ECPay"));

			builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
			builder.Services.AddTransient<IEmailSender, EmailSender>();


			builder.Services.AddTransient<
				Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,
				Cat_Paw_Footprint.Areas.CustomersArea.Services.CustomerEmailSender>();
			builder.Services.AddScoped<IChatAttachmentService, ChatAttachmentService>();


			var app = builder.Build();
			app.MapHub<ChatHub>("/chatHub");
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			// 必須在 authentication 之前啟用 Session Middleware
			app.UseSession();

			// 🌟 Claims→Session同步：每次 request 只要 claims 有就同步 Session（客戶/員工/供應商）
			app.Use(async (context, next) =>
			{
				var path = context.Request.Path.Value ?? "";
				// 員工區 Session 轉換
				if (path.StartsWith("/Employee", StringComparison.OrdinalIgnoreCase) ||
					path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
					path.StartsWith("/CouponManagement", StringComparison.OrdinalIgnoreCase) ||
					path.StartsWith("/ProductManagement", StringComparison.OrdinalIgnoreCase))
				{
					if (context.User?.Identity?.IsAuthenticated == true &&
						context.User.Identity.AuthenticationType == "EmployeeAuth")
					{
						var claims = context.User.Claims;
						var empId = claims.FirstOrDefault(c => c.Type == "EmployeeID")?.Value ?? "";
						if (!string.IsNullOrEmpty(empId))
							context.Session.SetString("EmpId", empId);
						context.Session.SetString("EmpRoleId", claims.FirstOrDefault(c => c.Type == "RoleID")?.Value ?? "");
						context.Session.SetString("EmpRoleName", claims.FirstOrDefault(c => c.Type == "RoleName")?.Value ?? "");
						context.Session.SetString("EmpName", claims.FirstOrDefault(c => c.Type == "EmployeeName")?.Value ?? "");
						context.Session.SetString("Status", claims.FirstOrDefault(c => c.Type == "Status")?.Value ?? "");
						context.Session.SetString("Login", "True");
					}
				}
				// 客戶區 Session 轉換
				if (path.StartsWith("/CustomersArea", StringComparison.OrdinalIgnoreCase))
				{
					if (context.User?.Identity?.IsAuthenticated == true &&
						context.User.Identity.AuthenticationType == "CustomerAuth")
					{
						var claims = context.User.Claims;
						var customerId = claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value ?? "";
						if (!string.IsNullOrEmpty(customerId))
							context.Session.SetString("CustomerID", customerId);
						context.Session.SetString("CustomerName", claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? "");
					}
				}
				// 供應商區 Session 轉換（如有供應商區域自行補充）
				// if (path.StartsWith("/Vendor", StringComparison.OrdinalIgnoreCase)) { ... }

				await next();
			});

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();
			app.MapHub<TicketChatHub>("/ticketChatHub");

			app.Run();
		}
	}
}