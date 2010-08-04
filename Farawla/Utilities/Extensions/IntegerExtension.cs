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
}
