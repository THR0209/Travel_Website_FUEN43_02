namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class SearchResultsVM
	{
		public SearchInput Input { get; set; }
		public string Sort { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public List<SearchResultItemVM> Items { get; set; }
		public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
	}
}
