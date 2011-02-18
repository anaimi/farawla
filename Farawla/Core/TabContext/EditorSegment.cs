using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Farawla.Core.TabContext
{
	public class EditorSegment
	{
		public Tab Tab { get; set; }
		public string Name { get; set; }
		public int Offset { get; set; }
		public int Length { get; set; }

		public EditorSegment(Tab tab, string name, int offset, int length)
		{
			Tab = tab;
			Name = name;
			Offset = offset;
			Length = length;
		}

		public EditorSegment(Tab tab)
		{
			Tab = tab;
			Name = tab.Language.Name;
			Offset = 0;
			Length = tab.Editor.Text.Length;
		}
		
		public string GetText()
		{
			return Tab.Editor.Document.GetText(Offset, Length);
		}
	}
}
