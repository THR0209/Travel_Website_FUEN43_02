using Microsoft.EntityFrameworkCore;

namespace testDBConnection.Models
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
		{
		}

		
		public DbSet<User> Users { get; set; }
		public DbSet<Customer> Customers { get; set; }

		public class User 
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string Email { get; set; }
		}


	}
}
