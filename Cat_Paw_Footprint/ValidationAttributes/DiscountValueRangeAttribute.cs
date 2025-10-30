using System;
using System.ComponentModel.DataAnnotations;
using Cat_Paw_Footprint.Areas.CouponManagement.ViewModel;

using System;
using System.ComponentModel.DataAnnotations;

public class DiscountValueRangeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 沒填值
        if (value == null)
            return new ValidationResult("折扣數值必須是數字");

        // 嘗試轉成數字
        if (!decimal.TryParse(value.ToString(), out decimal discount))
            return new ValidationResult("折扣數值必須是數字");

        // 取得 DiscountType 屬性
        var typeProperty = validationContext.ObjectType.GetProperty("DiscountType");
        if (typeProperty == null)
            return new ValidationResult("無法判斷折扣類型");

        var discountType = (int)typeProperty.GetValue(validationContext.ObjectInstance);

        // 百分比折扣：介於 0 與 1 之間
        if (discountType == 1)
        {
            if (discount <= 0 || discount > 1)
                return new ValidationResult("百分比折扣必須介於 0 與 1 之間（例如 0.9 代表九折）");
        }
        // 固定金額折扣：必須大於 0
        else if (discountType == 2)
        {
            if (discount <= 0)
                return new ValidationResult("固定金額折扣必須大於 0");
        }

        return ValidationResult.Success;
    }
}
