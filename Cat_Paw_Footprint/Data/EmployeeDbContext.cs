using Cat_Paw_Footprint.Models;
using Microsoft.EntityFrameworkCore;
namespace Cat_Paw_Footprint.Data
{
	public class EmployeeDbContext : DbContext
	{
		public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
			: base(options)
		{
		}
		public DbSet<Models.Employees> Employees { get; set; }
		public DbSet<Models.EmployeeRoles> EmployeeRoles { get; set; }
		public DbSet<Models.EmployeeProfile> EmployeeProfile { get; set; }
		public DbSet<Models.Customers> Customers { get; set; }
		public DbSet<Models.CustomerProfile> CustomerProfiles { get; set; }
		public DbSet<Models.CustomerLevels> CustomerLevels { get; set; }
		public DbSet<Models.CustomerLoginHistory> CustomerLoginHistory { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Employees>(entity =>
			{
				entity.HasKey(e => e.EmployeeID);

				// 多對一：Employee -> Role
				entity.HasOne(e => e.Role)
					  .WithMany(r => r.Employees)
					  .HasForeignKey(e => e.RoleID)
					  .OnDelete(DeleteBehavior.Restrict);

				// 一對一：Employee -> Profile（設定在這一邊就夠了）
				entity.HasOne(e => e.EmployeeProfile)
					  .WithOne(p => p.Employee)
					  .HasForeignKey<EmployeeProfile>(p => p.EmployeeID)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => e.RoleID); // 查詢常用可建索引
			});

			modelBuilder.Entity<EmployeeRoles>(entity =>
			{
				entity.HasKey(e => e.RoleID);
			});
			modelBuilder.Entity<EmployeeProfile>(entity =>
			{
				entity.HasKey(e => e.EmployeeProfileID);

				entity.HasIndex(p => p.EmployeeID).IsUnique(); // 1 對 1 的唯一鍵
			});



			modelBuilder.Entity<Customers>(entity =>
			{
				entity.HasKey(c => c.CustomerID);

				// 跟 Identity 連動 (Customer.UserId → AspNetUsers.Id)
				entity.HasOne(c => c.User)
					  .WithMany() // IdentityUser 不需要知道有多少 Customer
					  .HasForeignKey(c => c.UserId)
					  .OnDelete(DeleteBehavior.Restrict); // 避免刪 Identity 時 Cascade 全部 Customer
			});
			modelBuilder.Entity<Customers>()
	   .HasOne(c => c.LevelNavigation)
	   .WithMany(l => l.Customers)
	   .HasForeignKey(c => c.Level)   // 指定外鍵
	   .HasPrincipalKey(l => l.Level);// 指定主鍵
			modelBuilder.Entity<CustomerProfile>().ToTable("CustomerProfile", t => t.ExcludeFromMigrations());
			modelBuilder.Entity<CustomerLevels>().ToTable("CustomerLevels", t => t.ExcludeFromMigrations());
			modelBuilder.Entity<CustomerLoginHistory>().ToTable("CustomerLoginHistory", t => t.ExcludeFromMigrations());
		}

	}
}

