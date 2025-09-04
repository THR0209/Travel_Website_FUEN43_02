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
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Employees>(entity =>
			{
				entity.HasKey(e => e.EmployeeID);
			});

			modelBuilder.Entity<EmployeeRoles>(entity =>
			{
				entity.HasKey(e => e.RoleID);
			});
			modelBuilder.Entity<EmployeeProfile>(entity =>
			{
				entity.HasKey(e => e.EmployeeProfileID);
			});
			
		}

	}
}

