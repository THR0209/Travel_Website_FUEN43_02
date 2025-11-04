using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class SemiSelfProducts
{
	[Key]
	public int ProductID { get; set; }

	[DisplayName("產品名稱")]
    public string? ProductName { get; set; }

    public int? RegionID { get; set; }

	[DisplayName("產品描述")]
	public string? ProductDesc { get; set; }

	[DisplayName("產品價格")]
	public int? ProductPrice { get; set; }

	[DisplayName("產品類別")]
	public int? ProductType { get; set; } // 1 = 住宿，2 = 門票， 3 = 交通

	[DisplayName("上架時間")]
	public DateTime? StartDate { get; set; }

	[DisplayName("下架時間")]
	public DateTime? EndTime { get; set; }

	[DisplayName("建立時間")]
	public DateTime? CreateTime { get; set; }

	[DisplayName("更新時間")]
	public DateTime? UpdateTime { get; set; }

	[DisplayName("狀態")]
	public bool? IsActive { get; set; }  // 上下架狀態

	[DisplayName("瀏覽次數")]
	public int? Views { get; set; }

	[DisplayName("注意事項")]
	public string? Notes { get; set; }

	public int? MaxPeople { get; set; }

	[DisplayName("產品流水號")]
	public string? ProductCode { get; set; }

	public string? ProductImageUrl { get; set; }

	public virtual Regions? Region { get; set; }

	public virtual ICollection<Semi_Hotels> SemiHotels { get; set; } = new List<Semi_Hotels>();

    public virtual ICollection<Semi_Locations> SemiLocations { get; set; } = new List<Semi_Locations>();

    public virtual ICollection<Semi_Transportations> SemiTransportations { get; set; } = new List<Semi_Transportations>();

    public virtual ICollection<Semi_Keywords> SemiKeywords { get; set; } = new List<Semi_Keywords>();
}
