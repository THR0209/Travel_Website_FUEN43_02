using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat_Paw_Footprint.Models
{
	public partial class Favorites
	{
		[Key]
		public int FavoriteID { get; set; }

		// ==== 關聯欄位 ====
		[Required]
		public int CustomerID { get; set; }

		public int? ProductID { get; set; }

		//public int? SemiProductID { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// ==== 導覽屬性 ====
		[ForeignKey(nameof(CustomerID))]
		public virtual Customers Customer { get; set; }

		[ForeignKey(nameof(ProductID))]
		public virtual Products Product { get; set; }

		//[ForeignKey(nameof(SemiProductID))]
		//public virtual SemiSelfProducts SemiProduct { get; set; }
	}
}
