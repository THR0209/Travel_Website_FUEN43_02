namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class SearchResultItemVM
	{
		public int ProductID { get; set; }
		public string Name { get; set; }
		public string RegionName { get; set; }
		public int MinPrice { get; set; }
		public double Rating { get; set; }
		public string DurationText { get; set; }
		public string CoverImageUrl { get; set; }

		public List<string>? Keywords { get; set; }
		public string Source { get; set; } = "product";
	}
}
