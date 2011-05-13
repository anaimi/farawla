using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using Farawla.Utilities;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;

namespace Farawla.Core.TabContext
{
	class ColorPreviewRenderer : BaseRenderer, IBackgroundRenderer
	{
		private Tab tab;
		private List<ColorMatches> colors;
		private DocumentLine line;

		public KnownLayer Layer
		{
			get { return KnownLayer.Background; }
		}		
		
		public ColorPreviewRenderer(Tab tab)
		{
			colors = new List<ColorMatches>();
			
			this.tab = tab;

			tab.Editor.TextArea.Caret.PositionChanged += CaretPositionChanged;
		}

		private void CaretPositionChanged(object sender, EventArgs e)
		{
			colors.Clear();

			if (!Settings.Instance.ShowColorPreviewWhenCaretIsOnColorString)
				return;
			
			if (!tab.Editor.TextArea.Selection.IsEmpty)
				return;

			line = tab.Editor.Document.GetLineByOffset(tab.Editor.CaretOffset);
			
			var offset = tab.Editor.CaretOffset - line.Offset;
			var content = tab.Editor.Document.GetText(line);

			foreach (Match match in Regex.Matches(content, "\\#([A-Fa-f0-9]{6})([A-Fa-f0-9]{2})?"))
			{
				// check offset
				if (match.Index > offset || match.Index + match.Length < offset)
					continue;
				
				// add it
				colors.Add(new ColorMatches(match, match.Value.ToColor()));
			}
		}

		public void Draw(TextView textView, DrawingContext ctx)
		{
			if (line == null)
				return;
			
			foreach(var color in colors)
			{
				var from = GetPositionFromOffset(tab.Editor, VisualYPosition.LineTop, line.Offset + color.Match.Index);
				var to = GetPositionFromOffset(tab.Editor, VisualYPosition.LineTop, line.Offset + color.Match.Index + color.Match.Length);

				var x = from.X - tab.Editor.TextArea.TextView.ScrollOffset.X - 3;
				var y = from.Y - 18 - tab.Editor.TextArea.TextView.ScrollOffset.Y;
				var width = to.X - from.X + 12;
				var height = 16;
				
				ctx.DrawRoundedRectangle(new SolidColorBrush(color.Color), new Pen(), new Rect(x, y, width, height), 3, 3);
			}
		}
		
		private class ColorMatches
		{
			public Match Match { get; set; }
			public Color Color { get; set; }

			public ColorMatches(Match match, Color color)
			{
				Match = match;
				Color = color;
			}
		}
	}
}
