namespace Cat_Paw_Footprint.Areas.CustomersArea.ViewModel
{
	public class ProductDetailVM
	{
		// 基本資料
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public string RegionName { get; set; }
		public int ProductPrice { get; set; }
		public double Rating { get; set; }
		public string DurationText { get; set; }
		public string Category { get; set; }
		public string CoverImageUrl { get; set; } // 已處理成 /images/NoImage.png fallback
		public DateTime? ReleaseDate { get; set; }
		public DateTime? RemovalDate { get; set; }

		// 內容（HTML 可直接渲染）
		public string DescriptionHtml { get; set; }   // 產品描述
		public string NoteHtml { get; set; }          // 注意事項

		// 畫廊
		public List<string> ImageUrls { get; set; } = new();

		// 行程介紹 — 依你後台結構整理（來源參考：後台 Details View 的群組邏輯）  :contentReference[oaicite:2]{index=2}
		public List<DayLocationsDto> LocationsByDay { get; set; } = new();   // 景點（依 DayNumber 群組）
		public List<DayMealsDto> MealsByDay { get; set; } = new();           // 餐食（Breakfast/Lunch/Dinner）
		public List<DayHotelsDto> HotelsByDay { get; set; } = new();         // 住宿（依天）
		public List<TransportDto> Transports { get; set; } = new();          // 交通（列表）

		// DTOs
		public class DayLocationsDto
		{
			public int DayNumber { get; set; }
			public List<LocationDto> Items { get; set; } = new();
		}
		public class LocationDto
		{
			public string Name { get; set; }
			public string Desc { get; set; }
			public List<string> Pictures { get; set; } = new();
		}

		public class DayMealsDto
		{
			public int DayNumber { get; set; }
			public List<MealGroup> Meals { get; set; } = new(); // Breakfast/Lunch/Dinner
		}
		public class MealGroup
		{
			public string MealType { get; set; } // Breakfast | Lunch | Dinner
			public List<MealItem> Items { get; set; } = new();
		}
		public class MealItem
		{
			public string Name { get; set; }
			public string Desc { get; set; }
			public List<string> Pictures { get; set; } = new();
		}

		public class DayHotelsDto
		{
			public int DayNumber { get; set; }
			public List<HotelItem> Items { get; set; } = new();
		}
		public class HotelItem
		{
			public string Name { get; set; }
			public string Addr { get; set; }
			public string Desc { get; set; }
			public List<string> Pictures { get; set; } = new();
		}

		public class TransportDto
		{
			public string Name { get; set; }
			public string Desc { get; set; }
			public List<string> Pictures { get; set; } = new();
		}
	}
}
