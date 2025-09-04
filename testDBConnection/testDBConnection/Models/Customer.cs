namespace testDBConnection.Models
{
	public class Customer
	{
		public int? CustomerID { get; set; }
		public string? Account { get; set; }
		public string? Password { get; set; }
		public string? FullName { get; set; }
		public int? Level { get; set; }
		public DateTime? CreateDate { get; set; }
		public bool? Status { get; set; }
		public bool? IsBlacklisted { get; set; }
		public string? CustomerCode { get; set; }
	}
}
