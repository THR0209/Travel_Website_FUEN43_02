using System.ComponentModel.DataAnnotations;

namespace Cat_Paw_Footprint.ValidationAttributes
{
	public class DateRangeAttribute : ValidationAttribute
	{
		private readonly string _startProperty;
		private readonly string _endProperty;

		public DateRangeAttribute(string startProperty, string endProperty)
		{
			_startProperty = startProperty;
			_endProperty = endProperty;
		}
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			// 取得整個 ViewModel
			var instance = validationContext.ObjectInstance;

			var publishProp = validationContext.ObjectType.GetProperty("PublishTime");
			var expireProp = validationContext.ObjectType.GetProperty("ExpireTime");

			var publishValue = (DateTime?)publishProp?.GetValue(instance);
			var expireValue = (DateTime?)expireProp?.GetValue(instance);

			if (publishValue.HasValue && expireValue.HasValue && publishValue > expireValue)
			{
				return new ValidationResult("發佈時間不可大於到期時間");
			}

			return ValidationResult.Success;
		}
	}
}
