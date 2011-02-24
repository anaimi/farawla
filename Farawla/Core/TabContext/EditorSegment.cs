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
		
		public bool IsSyntax
		{
			get
			{
				return Name.EndsWith("-syntax");
			}
		}
		
		public string SyntaxName
		{
			get
			{
				return Name.Replace("-syntax", "");
			}
		}
		
		public string Text
		{
			get
			{
				return Tab.Editor.Document.GetText(Offset, Length);
			}
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = obj as EditorSegment;
			
			if (other.Name != Name) return false;
			if (other.Offset != Offset) return false;
			if (other.Length != Length) return false;

			return true;
		}
		
		public override int GetHashCode()
		{
			return Offset.GetHashCode();
		}
	}
}
