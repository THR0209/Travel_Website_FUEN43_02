using Cat_Paw_Footprint.Areas.Employee.Repositories;
using Cat_Paw_Footprint.Areas.Employee.Services;
using Cat_Paw_Footprint.Areas.Vendor.Repositories;
using Cat_Paw_Footprint.Areas.Vendor.Services;
using Cat_Paw_Footprint.Data;
using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
					opt.SignIn.RequireConfirmedAccount = false; // 測試先關掉信箱驗證
					opt.Password.RequiredLength = 6;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddDefaultUI();

			builder.Services.AddSession(options =>
			{
				options.Cookie.Name = ".CatPaw.Employee.Session"; // 自訂員工 Session 名稱
				options.IdleTimeout = TimeSpan.FromHours(9);   // 自訂逾時時間
				options.Cookie.HttpOnly = true;                   // 阻止 JS 存取，防止 XSS
				options.Cookie.IsEssential = true;                // 避免被瀏覽器阻擋
			});

			builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
			builder.Services.AddScoped<IEmployeeService, EmployeeService>();
			builder.Services.AddScoped<ICustomerAdminRepository, CustomerAdminRepository>();
			builder.Services.AddScoped<ICustomerAdminService, CustomerAdminService>();
			builder.Services.AddScoped<IVendorAdminRepository, VendorAdminRepository>();
			builder.Services.AddScoped<IVendorAdminService, VendorAdminService>();
			builder.Services.AddScoped<IVendorHomeRepository, VendorHomeRepository>();
			builder.Services.AddScoped<IVendorHomeService, VendorHomeService>();

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
			app.UseSession(); // 啟用 Session 中介軟體
			app.UseAuthentication();// 在 Authorization 之前
			app.UseAuthorization();
			app.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
			app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
