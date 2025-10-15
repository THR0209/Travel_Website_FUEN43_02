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
using Cat_Paw_Footprint.Models;
using ClosedXML.Parser;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Cat_Paw_Footprint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// 1️⃣ 載入 Google Cloud 金鑰 JSON
			var credential = GoogleCredential.FromFile(@"C:\GoogleCloudSql\Keys\web-travel-ap.json");

			// 2️⃣ 建立 Secret Manager Client
			var client = new SecretManagerServiceClientBuilder
			{
				Credential = credential
			}.Build();

			// 3️⃣ 讀取 Secret Manager 裡的連線字串
			var secretVersionName = new SecretVersionName("web-travel-473102", "sqlserver-connection", "latest");
			var result = client.AccessSecretVersion(secretVersionName);
			string connectionStringGoogleDB = result.Payload.Data.ToStringUtf8();

			Console.WriteLine($"✅ 從 Secret Manager 取得連線字串: {connectionStringGoogleDB}");

			// 註冊 DbContext (EF Core)
			builder.Services.AddDbContext<webtravel2Context>(options =>
				options.UseSqlServer(connectionStringGoogleDB));


			// Add services to the container.
			//builder.Services.AddDbContext<EmployeeDbContext>(options =>
			//	options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeConnection")));

			builder.Services.AddDbContext<EmployeeDbContext>(options =>
				options.UseSqlServer(connectionStringGoogleDB));

			//builder.Services.AddDbContext<webtravel2Context>(options =>
			//	options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeConnection")));

			builder.Services.AddDbContext<webtravel2Context>(options =>
				options.UseSqlServer(connectionStringGoogleDB));

			//var connectionString = builder.Configuration.GetConnectionString("EmployeeConnection") ?? throw new InvalidOperationException("Connection string 'EmployeeConnection' not found.");
			//builder.Services.AddDbContext<ApplicationDbContext>(options =>
			//	options.UseSqlServer(connectionString));

			var connectionString = builder.Configuration.GetConnectionString("EmployeeConnection") ?? throw new InvalidOperationException("Connection string 'EmployeeConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionStringGoogleDB));


			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services
				.AddIdentity<IdentityUser, IdentityRole>(opt => {
					opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//鎖定5分鐘
					opt.Lockout.MaxFailedAccessAttempts = 5;//5次錯誤就鎖定
					opt.Lockout.AllowedForNewUsers = true;//新用戶也鎖定


					opt.SignIn.RequireConfirmedAccount = false; // 註冊不需驗證
					opt.Password.RequiredLength = 6;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddDefaultUI();
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = "VendorAuth"; // �w�]�ϥ� VendorAuth
				options.DefaultChallengeScheme = "VendorAuth";
			})
	.AddCookie("VendorAuth", options =>
	{
		options.Cookie.Name = ".CatPaw.Vendor.Auth";
		options.LoginPath = "/Vendor/VendorHome/Login";   // 非登入時強制跳轉
		options.AccessDeniedPath = "/Vendor/VendorHome/Denied";// 非權限時強制跳轉
	})
	.AddCookie("CustomerAuth", options =>
	{
		options.Cookie.Name = ".CatPaw.Customer.Auth";
		options.LoginPath = "/CustomersArea/Account/Login"; // 非登入時強制跳轉
		options.AccessDeniedPath = "/CustomersArea/Account/Index";// 非權限時強制跳轉
	}).AddCookie("EmployeeAuth", options =>
	{
		options.Cookie.Name = ".CatPaw.Employee.Auth";
		options.LoginPath = "/Employee/EmployeeAuth/Login";      // 非登入時強制跳轉
		options.AccessDeniedPath = "/Home/Index";
	});

			#region AccessDeniedPath權限進入限制
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("Emp.AdminOnly", policy =>
					policy.AddAuthenticationSchemes("EmployeeAuth")
						  .RequireAuthenticatedUser()
						  .RequireClaim("RoleName", "Admin", "SuperAdmin"));

				options.AddPolicy("AreaAdmin", policy =>//���� �̷s���� �u�f����
		policy.AddAuthenticationSchemes("EmployeeAuth")
			  .RequireAuthenticatedUser()
			  .RequireClaim("RoleName", "Admin", "SuperAdmin"));

				options.AddPolicy("AreaCouponManagement", policy =>//�u�f��
		policy.AddAuthenticationSchemes("EmployeeAuth")
			  .RequireAuthenticatedUser()
			  .RequireClaim("RoleName", "ProductPlanner", "SuperAdmin", "Sales"));

				options.AddPolicy("AreaCustomerService", policy =>//�ȪA
		policy.AddAuthenticationSchemes("EmployeeAuth")
			  .RequireAuthenticatedUser()
			  .RequireClaim("RoleName", "CustomerService", "SuperAdmin"));

				options.AddPolicy("AreaOrder", policy =>//�q��
		policy.AddAuthenticationSchemes("EmployeeAuth")
			  .RequireAuthenticatedUser()
			  .RequireClaim("RoleName", "Sales", "SuperAdmin"));


				options.AddPolicy("AreaProductManagement", policy =>//���~
		policy.AddAuthenticationSchemes("EmployeeAuth")
			  .RequireAuthenticatedUser()
			  .RequireClaim("RoleName", "ProductPlanner", "SuperAdmin", "TourGuide"));

				options.AddPolicy("AreaTravelManagement", policy =>//�ȹC
		policy.AddAuthenticationSchemes("EmployeeAuth")
			  .RequireAuthenticatedUser()
			  .RequireClaim("RoleName", "TourGuide", "SuperAdmin", "ProductPlanner"));
			});
            builder.Services.AddDistributedMemoryCache();
            #endregion
            builder.Services.AddSession(options =>
			{
				options.Cookie.Name = ".CatPaw.Employee.Session"; // �ۭq���u Session �W��
				options.IdleTimeout = TimeSpan.FromHours(9);   // �ۭq�O�ɮɶ�
				options.Cookie.HttpOnly = true;                   // ���� JS �s���A���� XSS
				options.Cookie.IsEssential = true;                // �קK�Q�s��������
			});
			#region 註冊連線層與邏輯層
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
			builder.Services.AddScoped<ICustomerSupportTicketsRepository, CustomerSupportTicketsRepository>();
			builder.Services.AddScoped<ICustomerSupportTicketsService, CustomerSupportTicketsService>();
			builder.Services.AddScoped<ICustomerSupportFeedbackService, CustomerSupportFeedbackService>();
			builder.Services.AddScoped<ICustomerSupportFeedbackRepository, CustomerSupportFeedbackRepository>();
			builder.Services.AddScoped<ICustomerSupportMessagesRepository, CustomerSupportMessagesRepository>();
			builder.Services.AddScoped<ICustomerSupportMessagesService, CustomerSupportMessagesService>();
			builder.Services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
			builder.Services.AddScoped<IEmployeeMiniRepository, EmployeeMiniRepository>();
			builder.Services.AddScoped <ICusLogRegRepository, CusLogRegRepository>();
			builder.Services.AddScoped<ICusLogRegService, CusLogRegService>();
			builder.Services.AddScoped<ITGAllRepository, TGAllRepository>();
			builder.Services.AddScoped<ITGAllService, TGAllService>();
			#endregion

			builder.Services.AddHttpContextAccessor();

			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();
			builder.Services.AddSignalR();

			builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
			builder.Services.AddTransient<IEmailSender, EmailSender>();
			builder.Services.AddTransient<
	Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,
	Cat_Paw_Footprint.Areas.CustomersArea.Services.CustomerEmailSender>();

			var app = builder.Build();

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
			app.UseSession(); // �ҥ� Session �����n��
			app.Use(async (context, next) =>
			{
				var path = context.Request.Path.Value ?? "";

				if (path.StartsWith("/Employee", StringComparison.OrdinalIgnoreCase) ||
					path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
					path.StartsWith("/CouponManagement", StringComparison.OrdinalIgnoreCase) ||
					path.StartsWith("/ProductManagement", StringComparison.OrdinalIgnoreCase))
				{
					if (context.User?.Identity?.IsAuthenticated == true &&
						context.User.Identity.AuthenticationType == "EmployeeAuth" &&
						string.IsNullOrEmpty(context.Session.GetString("EmpId")))
					{
						var claims = context.User.Claims;
						context.Session.SetString("EmpId", claims.FirstOrDefault(c => c.Type == "EmployeeID")?.Value ?? "");
						context.Session.SetString("EmpRoleId", claims.FirstOrDefault(c => c.Type == "RoleID")?.Value ?? "");
						context.Session.SetString("EmpRoleName", claims.FirstOrDefault(c => c.Type == "RoleName")?.Value ?? "");
						context.Session.SetString("EmpName", claims.FirstOrDefault(c => c.Type == "EmployeeName")?.Value ?? "");
						context.Session.SetString("Status", claims.FirstOrDefault(c => c.Type == "Status")?.Value ?? "");
						context.Session.SetString("Login", "True");
					}
				}

				await next();
			});
			app.UseAuthentication();// �b Authorization ���e
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
