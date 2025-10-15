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
		//protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		//{
		//	// 取得整個 ViewModel
		//	var instance = validationContext.ObjectInstance;

		//	var publishProp = validationContext.ObjectType.GetProperty("PublishTime");
		//	var expireProp = validationContext.ObjectType.GetProperty("ExpireTime");

		//	var publishValue = (DateTime?)publishProp?.GetValue(instance);
		//	var expireValue = (DateTime?)expireProp?.GetValue(instance);

		//	if (publishValue.HasValue && expireValue.HasValue)
		//	{
		//		// 計算時間差
		//		var diff = expireValue.Value - publishValue.Value;

		//		if (diff < TimeSpan.FromHours(24))
		//		{
		//			return new ValidationResult("到期時間必須比發佈時間晚至少 24 小時");
		//		}
		//	}

		//	return ValidationResult.Success;
		//}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			// 取得整個 ViewModel instance
			var instance = validationContext.ObjectInstance;

			var startProp = validationContext.ObjectType.GetProperty(_startProperty);
			var endProp = validationContext.ObjectType.GetProperty(_endProperty);

			if (startProp == null || endProp == null)
			{
				return new ValidationResult($"找不到屬性 {_startProperty} 或 {_endProperty}");
			}

			var startValue = (DateTime?)startProp.GetValue(instance);
			var endValue = (DateTime?)endProp.GetValue(instance);

			if (startValue.HasValue && endValue.HasValue)
			{
				var diff = endValue.Value - startValue.Value;

				if (diff < TimeSpan.FromHours(24))
				{
					return new ValidationResult(
						ErrorMessage ?? $"{_endProperty} 必須比 {_startProperty} 晚至少 24 小時"
					);
				}
			}

			return ValidationResult.Success;
		}
	}
}
