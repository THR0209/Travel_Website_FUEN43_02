namespace Cat_Paw_Footprint.Models
{
    public class PromotionPics
    {
        public int PromotionPicID { get; set; }
        public int PromoID { get; set; }
        public string? PictureUrl { get; set; }
        public virtual Promotions Promotion { get; set; }
    }
}
