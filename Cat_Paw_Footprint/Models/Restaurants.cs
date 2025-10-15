using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Restaurants
{
	[Key]
	public int RestaurantID { get; set; }

	[Display(Name = "美食名稱")]
	public string? RestaurantName { get; set; }

	[Display(Name = "美食代碼")]
	public string? RestaurantCode { get; set; }

	[Display(Name = "美食地址")]
	public string? RestaurantAddr { get; set; }

	[Display(Name = "經度")]
	public decimal? RestaurantLat { get; set; }

	[Display(Name = "緯度")]
	public decimal? RestaurantLng { get; set; }

	[Display(Name = "美食介紹")]
	public string? RestaurantDesc { get; set; }

    public int? RegionID { get; set; }

    public int? DistrictID { get; set; }

	[Display(Name = "評分")]
	public decimal? Rating { get; set; }

	[Display(Name = "瀏覽數")]
	public int? Views { get; set; }

	[Display(Name = "是否啟用")]
	public bool? IsActive { get; set; }

	[Display(Name = "縣市")]
	public virtual Districts? District { get; set; }

	[Display(Name = "地區")]
	public virtual Regions? Region { get; set; }

	//一個美食可以有多個關鍵字、圖片

    public virtual ICollection<RestaurantKeywords> RestaurantKeywords { get; set; } = new List<RestaurantKeywords>();

	public virtual ICollection<RestaurantPics> RestaurantPics { get; set; } = new List<RestaurantPics>();
}
