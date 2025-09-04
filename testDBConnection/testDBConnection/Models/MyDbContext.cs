using Microsoft.EntityFrameworkCore;

namespace testDBConnection.Models
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
		{
		}

		
		
		public DbSet<Customer> Customers { get; set; }

		//public class Customer
		//{
		//	public int CustomerID { get; set; }
		//	public string Account { get; set; }
		//	public string Password { get; set; }
		//	public string FullName { get; set; }
		//	public int Level { get; set; }
		//	public DateTime CreateDate { get; set; }
		//	public bool Status { get; set; }
		//	public bool IsBlacklisted { get; set; }
		//	public string CustomerCode { get; set; }
		//}
		

	}
}
