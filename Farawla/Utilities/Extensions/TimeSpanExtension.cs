using System;
using System.Text;

public static class TimeSpanExtension
{
	public static string FriendlyTimeSpan(this TimeSpan diff)
	{
		var result = new StringBuilder();

		if (diff.Days > 0)
		{
			int days = diff.Days;
			int months = 0;
			int weeks = 0;

			if (days > 28)
			{
				if ((days / 29) == 1)
					result.AppendFormat("one month");
				else if ((days / 29) > 1)
					result.AppendFormat("{0} months", (days / 29));

				months = (days / 29);
				days -= 29 * (days / 29);
			}

			if (days > 6)
			{
				if (result.Length > 0)
				{
					result.Append(" and ");
				}

				if ((days / 7) == 1)
					result.AppendFormat("one week");
				else if ((days / 7) > 1)
					result.AppendFormat("{0} weeks", (days / 7));

				weeks = (days / 7);
				days -= 7 * (days / 7);
			}

			if (((months == 0) || (weeks == 0)) && (days > 0))
			{
				if (result.Length > 0)
				{
					result.Append(" and ");
				}

				if (days == 1)
					result.AppendFormat("one day");
				else if (days > 1)
					result.AppendFormat("{0} days", days);
			}
		}

		if ((diff.Days <= 6) && (diff.Hours > 0))
		{
			if (result.Length > 0)
			{
				result.Append(" and ");
			}

			if (diff.Hours == 1)
				result.AppendFormat("one hour");
			else
				result.AppendFormat("{0} hours", diff.Hours);
		}

		if ((diff.Days == 0) && (diff.Minutes > 0))
		{
			if (result.Length > 0)
			{
				result.Append(" and ");
			}

			if (diff.Minutes == 1)
				result.AppendFormat("one minute");
			else
				result.AppendFormat("{0} minutes", diff.Minutes);
		}

		if (result.Length == 0)
		{
			result.Append("few seconds");
		}

		return result.ToString();
	}
}