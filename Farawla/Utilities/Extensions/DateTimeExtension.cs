using System;

public static class DateTimeExtension
{
	public static string Ago(this DateTime target)
	{
		TimeSpan diff = (DateTime.Now - target);

		return diff.FriendlyTimeSpan() + " ago";
	}

	public static string ToPragmaticDate(this DateTime date)
	{
		return date.ToString("dd-MMMM-yyyy-@-hh-mm-ss-tt");
	}
}