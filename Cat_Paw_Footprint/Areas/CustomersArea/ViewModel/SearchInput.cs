using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class SearchInput
	{
		[Required] public string Destination { get; set; }

		[Required, DataType(DataType.Date)]
		public DateTime? Start { get; set; }

		[Required, DataType(DataType.Date)]
		public DateTime? End { get; set; }

		public int People { get; set; } = 2;

		// trip / hotel / ticket / car
		public string Type { get; set; } = "trip";

		public List<string>? Departures { get; set; }


		// 側邊篩選
		[Range(0, double.MaxValue)] public decimal? MinPrice { get; set; }
		[Range(0, double.MaxValue)] public decimal? MaxPrice { get; set; }
		[Range(0, 5)] public double? MinRating { get; set; }

		// 若有天數欄位再開
		// public int? MinDays { get; set; }
		// public int? MaxDays { get; set; }
	}
}
