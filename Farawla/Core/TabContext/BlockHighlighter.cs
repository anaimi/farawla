using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using DrawingContext = System.Windows.Media.DrawingContext;

namespace Farawla.Core.TabContext
{
	public class BlockHighlighter : IBackgroundRenderer
	{
		private List<HighlightedBlocks> blocks;
		private TextEditor editor;

		private SolidColorBrush tabsColor;
		private SolidColorBrush spacesColor;
		private SolidColorBrush lineOfCaret;

		public BlockHighlighter(TextEditor editor)
		{
			this.editor = editor;

			blocks = new List<HighlightedBlocks>();

			tabsColor = new SolidColorBrush(Theme.Instance.TabColor.ToColor());
			spacesColor = new SolidColorBrush(Theme.Instance.SpaceColor.ToColor());

			if (Theme.Instance.HighlightLineOfCaret)
			{
				lineOfCaret = new SolidColorBrush(Theme.Instance.LineOfCaretColor.ToColor());
			}
		}

		public KnownLayer Layer
		{
			get { return KnownLayer.Background; }
		}

		public void Add(string owner, int offset, int length, Color color)
		{
			blocks.Add(new HighlightedBlocks(owner, offset, length, color));
		}

		public void Remove(string owner, int offset, int length)
		{
			var block = blocks.FirstOrDefault(b => b.Owner == owner && b.Offset == offset && b.Length == length);
			blocks.Remove(block);
		}

		public void ClearAll()
		{
			blocks.Clear();
		}

		public void Clear(string owner)
		{
			blocks.RemoveAll(b => b.Owner == owner);
		}

		public void Draw(TextView textView, DrawingContext ctx)
		{
			textView.EnsureVisualLines();

			//TODO: probably should only draw lines inside the view area (i.e. inside the fold)

			// draw tabs
			if (Settings.Instance.ShowTabsInEditor)
			{
				foreach (Match match in Regex.Matches(editor.Text, "\t"))
				{
					var point = GetPositionFromOffset(textView, VisualYPosition.LineMiddle, match.Index);

					var x1 = point.X - editor.TextArea.TextView.ScrollOffset.X + 3;
					var y1 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					var x2 = point.X - editor.TextArea.TextView.ScrollOffset.X + 23;
					var y2 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 == x2)
						continue;

					var pen = new Pen
					{
						Brush = tabsColor,
						Thickness = 0.3
					};

					ctx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
				}
			}

			// draw spaces
			if (Settings.Instance.ShowSpacesInEditor)
			{
				foreach (Match match in Regex.Matches(editor.Text, " "))
				{
					var point = GetPositionFromOffset(textView, VisualYPosition.LineMiddle, match.Index);

					var x1 = point.X - editor.TextArea.TextView.ScrollOffset.X + 2;
					var y1 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					var x2 = point.X - editor.TextArea.TextView.ScrollOffset.X + 4;
					var y2 = point.Y - editor.TextArea.TextView.ScrollOffset.Y;

					if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 == x2)
						continue;

					var pen = new Pen
					{
						Brush = spacesColor,
						Thickness = 1
					};

					ctx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
				}
			}

			// draw highlighted line
			if (Theme.Instance.HighlightLineOfCaret && editor.TextArea.IsFocused)
			{
				var line = editor.Document.GetLineByOffset(editor.CaretOffset);
				var start = GetPositionFromOffset(textView, VisualYPosition.LineTop, line.Offset);
				var end = GetPositionFromOffset(textView, VisualYPosition.LineBottom, line.Offset);

				ctx.DrawRoundedRectangle(lineOfCaret, new Pen(), new Rect(start.X, start.Y - editor.TextArea.TextView.ScrollOffset.Y, textView.ActualWidth, end.Y - start.Y), 3, 3);
			}

			// draw blocks
			foreach (var block in blocks)
			{
				var startPosition = GetPositionFromOffset(textView, VisualYPosition.LineTop, block.Offset);
				var endPosition = GetPositionFromOffset(textView, VisualYPosition.LineBottom, block.Offset + block.Length);

				var x = startPosition.X - editor.TextArea.TextView.ScrollOffset.X;
				var y = startPosition.Y - editor.TextArea.TextView.ScrollOffset.Y;

				var width = endPosition.X - startPosition.X;
				var height = endPosition.Y - startPosition.Y;

				if (width <= 0 || height <= 0)
					continue;

				ctx.DrawRoundedRectangle(new SolidColorBrush(block.Color), new Pen(), new Rect(x, y, width, height), 3, 3);
			}
		}

		private Point GetPositionFromOffset(TextView textView, VisualYPosition position, int offset)
		{
			var startLocation = textView.Document.GetLocation(offset);
			var point = textView.GetVisualPosition(new TextViewPosition(startLocation), position);

			return new Point(Math.Round(point.X), Math.Round(point.Y));
		}

		public void Redraw()
		{
			editor.TextArea.TextView.Redraw();
		}
	}

	public class HighlightedBlocks
	{
		public string Owner { get; set; }
		public int Offset { get; set; }
		public int Length { get; set; }
		public Color Color { get; set; }

		public HighlightedBlocks(string owner, int offset, int length, Color color)
		{
			Owner = owner;
			Offset = offset;
			Length = length;
			Color = color;
		}
	}
}
