using Cat_Paw_Footprint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
	    }
		public DbSet<Vendors> Vendors { get; set; } = null!;
		public DbSet<Customers> Customers { get; set; } = null!;
		public DbSet<Models.CustomerProfile> CustomerProfiles { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Vendors>()
				.HasOne(v => v.User)
				.WithOne() // IdentityUser 不需要反向導航
				.HasForeignKey<Vendors>(v => v.UserId);
		}
	}
}
