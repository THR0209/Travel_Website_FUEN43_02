using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Cat_Paw_Footprint.Models;

public partial class Hotels
{
	[Key]
	public int HotelID { get; set; }

    [Display(Name="住宿名稱")]
    public string? HotelName { get; set; }

	[Display(Name = "住宿地址")]
	public string? HotelAddr { get; set; }

	[Display(Name = "經度")]
	public decimal? HotelLat { get; set; }

    [Display(Name = "緯度")]
	public decimal? HotelLng { get; set; }

    [Display(Name = "住宿介紹")]
	public string? HotelDesc { get; set; }
	
	public int? RegionID { get; set; }
	
	public int? DistrictID { get; set; }

	[Display(Name = "評分")]
	public decimal? Rating { get; set; }

	[Display(Name = "瀏覽數")]
	public int? Views { get; set; }

	[Display(Name = "住宿代碼")]
	public string? HotelCode { get; set; }

	[Display(Name = "是否啟用")]
	public bool? IsActive { get; set; }

	[Display(Name = "縣市")]
	public virtual Districts? District { get; set; }

	[Display(Name = "地區")]
	public virtual Regions? Region { get; set; }

	//一個住宿可以有多個圖片、關鍵字
	public virtual ICollection<HotelPics> HotelPics { get; set; } = new List<HotelPics>();

    public virtual ICollection<HotelKeywords> HotelKeywords { get; set; } = new List<HotelKeywords>();
}
