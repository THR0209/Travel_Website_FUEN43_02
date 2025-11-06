namespace Cat_Paw_Footprint.Areas.ProductManagement.Services
{
	public class TzHelper
	{
		public static TimeZoneInfo GetTaipeiTimeZone()
		{
			try { return TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); }
			catch { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei"); }
		}

		public static DateTime? ToUtcFromTaipei(DateTime? local)
		{
			if (!local.HasValue) return null;
			var tz = GetTaipeiTimeZone();
			return TimeZoneInfo.ConvertTimeToUtc(
				DateTime.SpecifyKind(local.Value, DateTimeKind.Unspecified),
				tz);
		}

		public static DateTime? ToTaipeiFromUtc(DateTime? utc)
		{
			if (!utc.HasValue) return null;
			var tz = GetTaipeiTimeZone();
			return TimeZoneInfo.ConvertTimeFromUtc(
				DateTime.SpecifyKind(utc.Value, DateTimeKind.Utc),
				tz);
		}
	}
}
