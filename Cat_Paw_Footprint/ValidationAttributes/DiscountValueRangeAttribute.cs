using System;
using System.ComponentModel.DataAnnotations;

public class DiscountValueRangeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 檢查折扣數值是否為空
        if (value == null)
            return new ValidationResult("折扣數值必須是數字");

        // 嘗試轉換為數字
        if (!decimal.TryParse(value.ToString(), out decimal discount))
            return new ValidationResult("折扣數值必須是數字");

        // 取得 DiscountType 屬性
        var typeProperty = validationContext.ObjectType.GetProperty("DiscountType");
        if (typeProperty == null)
            return new ValidationResult("無法判斷折扣類型");

        // 獲取 DiscountType 的值
        var discountType = (int)typeProperty.GetValue(validationContext.ObjectInstance);

        // 取得 MaximumDiscount 屬性 (這是固定金額折扣的最大限制)
        var maxDiscountProperty = validationContext.ObjectType.GetProperty("MaximumDiscount");
        var maxValueObj = maxDiscountProperty?.GetValue(validationContext.ObjectInstance);
        decimal maxDiscount = maxValueObj != null ? Convert.ToDecimal(maxValueObj) : decimal.MaxValue;

        // 檢查百分比折扣的範圍（0-1 之間）
        if (discountType == 1)
        {
            if (discount <= 0 || discount > 1)
                return new ValidationResult("百分比折扣必須介於 0 與 1 之間（例如 0.9 代表九折）");
        }
        // 檢查固定金額折扣的範圍（必須大於 0 且小於等於最大折扣額）
        else if (discountType == 2)
        {
            if (discount <= 0)
                return new ValidationResult("固定金額折扣必須大於 0");

            if (discount > maxDiscount)
                return new ValidationResult($"固定金額折扣不能大於最大折扣額 {maxDiscount} 元");
        }
        else
        {
            return new ValidationResult("未知的折扣類型");
        }

        return ValidationResult.Success;
    }
}
