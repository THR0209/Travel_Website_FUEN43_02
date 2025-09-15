using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.Models;

public partial class Products
{
	[Key]
	public int ProductID { get; set; }

	[Required(ErrorMessage = "請輸入產品名稱")]
    [DisplayName("產品名稱")]
    public string? ProductName { get; set; }

	[DisplayName("地區")]
	public int? RegionID { get; set; }

	[DisplayName("產品描述")]
	public string? ProductDesc { get; set; }

	[DisplayName("產品價格")]
	public int? ProductPrice { get; set; }

	[DisplayName("備註")]
	public string? ProductNote { get; set; }

	[DisplayName("出發日期")]
	public DateTime? StartDate { get; set; }

	[DisplayName("結束日期")]
	public DateTime? EndDate { get; set; }

	[DisplayName("人數上限")]
	public int? MaxPeople { get; set; }

	[DisplayName("狀態")]
	public bool? IsActive { get; set; }

	[DisplayName("瀏覽次數")]
	public int? Views {  get; set; }

	public byte[]? ProductImage { get; set; }

	[DisplayName("產品編號")]
	public string? ProductCode { get; set; }

    public virtual ICollection<CustomerOrders> CustomerOrders { get; set; } = new List<CustomerOrders>();

    public virtual Regions? Region { get; set; }

	// 一對一或一對多關聯都能用
	public virtual ICollection<ProductAnalysis> ProductAnalyses { get; set; } = new List<ProductAnalysis>();

	public virtual ICollection<ProductPics> ProductPics { get; set; } = new List<ProductPics>();

	public virtual ICollection<Products_Hotels> ProductHotels { get; set; } = new List<Products_Hotels>();

	public virtual ICollection<Products_Locations> ProductLocations { get; set; } = new List<Products_Locations>();

	public virtual ICollection<Products_Restaurants> ProductRestaurants { get; set; } = new List<Products_Restaurants>();

	public virtual ICollection<Products_Transportations> ProductsTransportations { get; set; } = new List<Products_Transportations>();


}
