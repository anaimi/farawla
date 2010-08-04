using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class DoubleExtension
{
	public static double Round(this double value, int decimals)
	{
		return Math.Round(value, decimals);
	}
	
	public static bool IsBetween(this double value, double min, double max)
	{
		if (value < min)
			return false;
		
		if (value > max)
			return false;
		
		return true;
	}
}
