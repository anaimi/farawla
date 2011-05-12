using System;
using System.Windows.Controls;

namespace Farawla.Utilities
{
	public class ContextMenuHelper
	{
		public static MenuItem CreateManuItem(string header, string shortcut, Action action)
		{
			var item = new MenuItem();

			item.Header = header;
			item.InputGestureText = shortcut;
			item.Click += (s, e) => action();

			return item;
		}
	}
}