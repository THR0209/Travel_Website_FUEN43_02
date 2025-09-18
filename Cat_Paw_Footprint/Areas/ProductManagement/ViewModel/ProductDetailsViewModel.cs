using Cat_Paw_Footprint.Models;

namespace Cat_Paw_Footprint.Areas.ProductManagement.ViewModel
{
	public class ProductDetailsViewModel
	{
		public Products Product { get; set; }
		public List<Products_Hotels> Hotels { get; set; }
		public List<Products_Restaurants> Restaurants { get; set; }
		public List<Products_Transportations> Transports { get; set; }
		public List<Products_Locations> Locations { get; set; }
		public List<ProductAnalysis> Analyses { get; set; }
	}
}
