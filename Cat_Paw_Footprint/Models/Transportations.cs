using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Transportations
{
	[Key]
	public int TransportID { get; set; }

	[Display(Name = "交通名稱")]
	public string? TransportName { get; set; }

	[Display(Name = "交通介紹")]
	public string? TransportDesc { get; set; }

	[Display(Name = "交通價位")]
	public int? TransportPrice { get; set; }

	[Display(Name = "評分")]
	public decimal? Rating { get; set; }

	[Display(Name = "瀏覽數")]
	public int? Views { get; set; }

	[Display(Name = "交通代碼")]
	public string? TransportCode { get; set; }

	[Display(Name = "是否啟用")]
	public bool? IsActive { get; set; }

	//一個交通可以有多個關鍵字、圖片
	public virtual ICollection<TransportKeywords> TransportKeywords { get; set; } = new List<TransportKeywords>();

	public virtual ICollection<TransportPics> TransportPics { get; set; } = new List<TransportPics>();
}
