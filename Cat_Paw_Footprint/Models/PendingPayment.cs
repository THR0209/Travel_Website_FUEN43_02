using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PendingPayments")]
public class PendingPayment
{
	[Key]
	public int PendingPaymentId { get; set; }       // ← 對齊你的 PK

	[Required, MaxLength(100)]
	public string SnapKey { get; set; } = "";

	public int CustomerId { get; set; }

	public string ItemsJson { get; set; } = "";

	// ← 對齊你的欄位名
	public int TotalAmount { get; set; }

	public string? CouponJson { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.Now;

	/// <summary>0=pending, 1=paid, 2=expired</summary>
	public byte Status { get; set; } = 0;

	// 方便程式使用的糖衣：不用儲存到 DB（可選）
	[NotMapped]
	public bool IsConsumed
	{
		get => Status == 1;
		set => Status = (byte)(value ? 1 : 0);
	}
}