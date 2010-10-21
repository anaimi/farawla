using System;
using System.Text;
using System.Web;
using System.Windows.Media;

public static class StringExtension
{
	public static string ToBase64(this string str)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
	}

	public static string FromBase64(this string str)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(str));
	}

	public static double ToDouble(this string str)
	{
		double d = 0;

		if (double.TryParse(str, out d))
			return d;

		throw new Exception("Cast Exception");
	}

	public static int ToInteger(this string str)
	{
		int d = 0;

		if (int.TryParse(str, out d))
			return d;

		throw new Exception("Cast Exception");
	}

	public static DateTime ToDateTime(this string str)
	{
		return DateTime.Parse(str);
	}

	public static bool IsInteger(this string str)
	{
		int n = 0;

		return int.TryParse(str, out n);
	}

	public static bool IsDouble(this string str)
	{
		double n = 0;

		return double.TryParse(str, out n);
	}

	/// <summary>
	/// Use it to find out if the string is part of a collection.
	/// </summary>
	public static bool IsOneOf(this string str, params string[] collection)
	{
		foreach (var s in collection)
		{
			if (str == s)
				return true;
		}

		return false;
	}

	public static string Chop(this string str, int maxLength)
	{
		if (str.Length > maxLength)
		{
			return str.Substring(0, maxLength - 3) + "...";
		}

		return str;
	}

	public static string GetPragmaticString(this string str)
	{
		var pragmaticName = new StringBuilder();

		foreach (var c in str)
		{
			if (!char.IsLetterOrDigit(c))
				continue;

			pragmaticName.Append(c);
		}

		return pragmaticName.ToString();
	}

	public static bool IsBlank(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return true;

		foreach (var c in str)
			if (!char.IsWhiteSpace(c))
				return false;

		return true;
	}

	public static string Extract(this string str, string begin, string end)
	{
		if (begin.IsBlank())
		{
			return str.Substring(0, str.IndexOf(end));
		}

		return str.Substring(str.IndexOf(begin), str.Length - str.IndexOf(end));
	}
	
	public static Color ToColor(this string str)
	{
		int a, r, g, b;
		
		a = 0;
		r = 0;
		g = 0;
		b = 0;
		
		if (!str.StartsWith("#"))
			throw new Exception("Color string must start with a '#'");
		
		if (str.Length != 7 && str.Length != 9)
			throw new Exception("String '" + str + "' is not a valid 6- or 8-digits hex color");

		if (str.Length == 7)
		{
			r = Convert.ToInt32(str.Substring(1, 2), 16);
			g = Convert.ToInt32(str.Substring(3, 2), 16);
			b = Convert.ToInt32(str.Substring(5, 2), 16);

			return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
		}

		a = Convert.ToInt32(str.Substring(1, 2), 16);
		r = Convert.ToInt32(str.Substring(3, 2), 16);
		g = Convert.ToInt32(str.Substring(5, 2), 16);
		b = Convert.ToInt32(str.Substring(7, 2), 16);

		return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
	}
	
	public static int GetLevenshteinDistance(this string str, string other)
	{
		var n = str.Length;
        var m = other.Length;
        
        int[,] d = new int[n + 1, m + 1];
 
        // Step 1
        if (n == 0)
        {
            return m;
        }
 
        if (m == 0)
        {
            return n;
        }
 
        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }
 
        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }
 
        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
				int cost = (other[j - 1] == str[i - 1]) ? 0 : 1;
 
                // Step 6
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        
        // Step 7
        return d[n, m];
	}
}