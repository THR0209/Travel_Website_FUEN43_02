using Cat_Paw_Footprint.Areas.CustomerService.Repositories;
using Cat_Paw_Footprint.Areas.CustomerService.Services;
using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.Services;
using Cat_Paw_Footprint.Areas.Vendor.Repositories;
using Cat_Paw_Footprint.Areas.Vendor.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cat_Paw_Footprint.Areas.Order.Models;
using Cat_Paw_Footprint.Areas.Order.Services;

namespace Cat_Paw_Footprint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<EmployeeDbContext>(options =>
	            options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeConnection")));

			builder.Services.AddDbContext<webtravel2Context>(options =>
	            options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeConnection")));

			var connectionString = builder.Configuration.GetConnectionString("EmployeeConnection") ?? throw new InvalidOperationException("Connection string 'EmployeeConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services
				.AddIdentity<IdentityUser, IdentityRole>(opt => {
					opt.SignIn.RequireConfirmedAccount = false; // ���ե������H�c����
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
		options.LoginPath = "/Vendor/VendorHome/Login";   // �t�ӵn�J��
		options.AccessDeniedPath = "/Vendor/VendorHome/Denied";
	})
	.AddCookie("CustomerAuth", options =>
	{
		options.Cookie.Name = ".CatPaw.Customer.Auth";
		options.LoginPath = "/Customer/Account/Login"; // �Ȥ�n�J��
		options.AccessDeniedPath = "/Customer/Account/Denied";
	}).AddCookie("EmployeeAuth", options =>
	{
		options.Cookie.Name = ".CatPaw.Employee.Auth";
		options.LoginPath = "/Employee/EmployeeAuth/Login";       // ���u�n�J��
		options.AccessDeniedPath = "/Home/Index";
	});

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

			builder.Services.AddSession(options =>
			{
				options.Cookie.Name = ".CatPaw.Employee.Session"; // �ۭq���u Session �W��
				options.IdleTimeout = TimeSpan.FromHours(9);   // �ۭq�O�ɮɶ�
				options.Cookie.HttpOnly = true;                   // ���� JS �s���A���� XSS
				options.Cookie.IsEssential = true;                // �קK�Q�s��������
			});

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

			builder.Services.AddHttpContextAccessor();

			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();

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

            app.Run();
			builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
			builder.Services.AddTransient<IEmailSender, EmailSender>();
		}
    }
}
