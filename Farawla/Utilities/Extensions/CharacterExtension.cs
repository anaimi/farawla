using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CharacterExtension
{
	public static bool IsOneOf(this char _char, params char[] collection)
	{
		foreach (var c in collection)
		{
			if (_char == c)
				return true;
		}

		return false;
	}
}
