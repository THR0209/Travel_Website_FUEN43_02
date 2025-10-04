using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Microsoft.EntityFrameworkCore;


namespace testDBConnection
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
			builder.Services.AddDbContext<Models.MyDbContext>(options =>
				options.UseSqlServer(connectionStringGoogleDB));

			builder.Services.AddDbContext<Models.MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddHttpClient();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Test}/{action=CheckDb}/{id?}");

            app.Run();
        }
    }
}
