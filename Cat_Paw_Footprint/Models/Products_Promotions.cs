using System;
using System.Collections.Generic;

namespace Cat_Paw_Footprint.Models;

public partial class Products_Promotions
{
    public int? PromoID { get; set; }

    public int? ProductID { get; set; }

    public virtual Products? Product { get; set; }

    public virtual Promotions? Promo { get; set; }
}
