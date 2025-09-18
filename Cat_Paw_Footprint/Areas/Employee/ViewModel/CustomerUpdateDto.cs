namespace Cat_Paw_Footprint.Areas.Employee.ViewModel
{
	public class CustomerUpdateDto
	{
		public int CustomerId { get; set; }
		public int LevelId { get; set; }
		public bool Status { get; set; }
		public bool IsBlacklisted { get; set; }
	}
}
