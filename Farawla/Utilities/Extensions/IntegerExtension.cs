using System;

public static class IntegerExtension
{
	public static string Pluralize(this int number, string single, string plural)
	{
		if (number == 1)
			return "one " + single;

		if (number == 0)
			return "no " + plural;

		return number + " " + plural;
	}

	public static string GetShortForm(this int number)
	{
		if (number >= 1000)
			return (number / 1000) + "k";

		return number.ToString();
	}

	public static bool IsBetween(this int value, double min, double max)
	{
		if (value < min)
			return false;

		if (value > max)
			return false;

		return true;
	}

	public static string ToPrettyBytes(this int bytes)
	{
		const int scale = 1024;
		string[] orders = new[] { "GB", "MB", "KB", "Bytes" };
		long max = (long)Math.Pow(scale, orders.Length - 1);
		
		foreach (string order in orders)
		{
			if (bytes > max)
				return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

			max /= scale;
		}
		
		return "0 Bytes";
	}
}
