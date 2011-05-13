using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace Farawla.Core.TabContext
{
	public class BaseRenderer
	{
		public Tab Tab { get; set; }
		
		public Point GetPositionFromOffset(TextEditor editor, VisualYPosition position, int offset)
		{
			var startLocation = editor.TextArea.TextView.Document.GetLocation(offset);
			var point = editor.TextArea.TextView.GetVisualPosition(new TextViewPosition(startLocation), position);

			return new Point(Math.Round(point.X), Math.Round(point.Y));
		}
	}
}
