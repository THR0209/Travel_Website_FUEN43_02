using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Locations
{
	[Key]
	public int LocationID { get; set; }

    [Display(Name ="景點名稱")]
    public string? LocationName { get; set; }

    [Display(Name = "景點地址")]
	public string? LocationAddr { get; set; }

	[Display(Name = "經度")]
	public decimal? LocationLat { get; set; }

    [Display(Name = "緯度")]
	public decimal? LocationLng { get; set; }

	[Display(Name = "景點介紹")]
	public string? LocationDesc { get; set; }

	[Display(Name = "門票費用")]
	public int? LocationPrice { get; set; }

    public int? RegionID { get; set; }

    public int? DistrictID { get; set; }

	[Display(Name = "評分")]
	public decimal? Rating { get; set; }
	
	[Display(Name = "瀏覽數")]
	public int? Views { get; set; }

	[Display(Name = "景點代碼")]
	public string? LocationCode { get; set; }

	[Display(Name = "是否啟用")]
	public bool? IsActive { get; set; }

	[Display(Name = "縣市")]
	public virtual Districts? District { get; set; }

	[Display(Name = "地區")]
	public virtual Regions? Region { get; set; }

	//一個景點可以有多個圖片、關鍵字
	public virtual ICollection<LocationPics> LocationPics { get; set; } = new List<LocationPics>();

    public virtual ICollection<LocationKeywords> LocationKeywords { get; set; } = new List<LocationKeywords>();
}
