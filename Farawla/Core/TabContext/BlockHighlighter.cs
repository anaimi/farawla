using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
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

		private DocumentLine viewportFirstLine;
		private DocumentLine viewportLastLine;

		public BlockHighlighter(TextEditor editor)
		{
			this.editor = editor;

			blocks = new List<HighlightedBlocks>();

			tabsColor = new SolidColorBrush(Theme.Instance.ShowTabColor.ToColor());
			spacesColor = new SolidColorBrush(Theme.Instance.ShowSpaceColor.ToColor());

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
			
			// update first and last line
			viewportFirstLine = editor.TextArea.TextView.GetDocumentLineByVisualTop(editor.TextArea.TextView.ScrollOffset.Y);
			viewportLastLine = editor.TextArea.TextView.GetDocumentLineByVisualTop(editor.TextArea.TextView.ScrollOffset.Y + editor.ActualHeight);

			#region Draw tabs

			if (Settings.Instance.ShowTabsInEditor)
			{
				foreach (Match match in Regex.Matches(editor.Text, "\t"))
				{
					if (!IsOffsetInsideViewport(match.Index, match.Index + 1))
						continue;
					
					var rects = BackgroundGeometryBuilder.GetRectsForSegment(editor.TextArea.TextView, new TextSegment {
						StartOffset = match.Index,
						Length = 1,
					});
					
					if (rects.Count() == 0)
						continue;
					
					var rect = rects.First();
					var x1 = rect.X + 3;
					var y1 = rect.Y + rect.Height / 2;

					var x2 = rect.X + rect.Width - 3;
					var y2 = y1;

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
			
			#endregion

			#region Draw spaces
			
			if (Settings.Instance.ShowSpacesInEditor)
			{
				foreach (Match match in Regex.Matches(editor.Text, " "))
				{
					if (!IsOffsetInsideViewport(match.Index, match.Index + 1))
						continue;
					
					var point = GetPositionFromOffset(VisualYPosition.LineMiddle, match.Index);

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
			
			#endregion

			#region Draw highlighted line
			
			if (Theme.Instance.HighlightLineOfCaret && editor.TextArea.IsFocused)
			{
				var line = editor.Document.GetLineByOffset(editor.CaretOffset);
				var start = GetPositionFromOffset(VisualYPosition.LineTop, line.Offset);
				var end = GetPositionFromOffset(VisualYPosition.LineBottom, line.Offset);

				ctx.DrawRoundedRectangle(lineOfCaret, new Pen(), new Rect(start.X, start.Y - editor.TextArea.TextView.ScrollOffset.Y, textView.ActualWidth, end.Y - start.Y), 3, 3);
			}
			
			#endregion

			#region Draw blocks
			
			foreach (var block in blocks)
			{
				if (!IsOffsetInsideViewport(block.Offset, block.Offset + block.Length))
					continue;
				
				var startPosition = GetPositionFromOffset(VisualYPosition.LineTop, block.Offset);
				var endPosition = GetPositionFromOffset(VisualYPosition.LineBottom, block.Offset + block.Length);

				var x = startPosition.X - editor.TextArea.TextView.ScrollOffset.X;
				var y = startPosition.Y - editor.TextArea.TextView.ScrollOffset.Y;

				var width = endPosition.X - startPosition.X;
				var height = endPosition.Y - startPosition.Y;
				
				if (width <= 0 || height <= 0)
					continue;
				
				ctx.DrawRoundedRectangle(new SolidColorBrush(block.Color), new Pen(), new Rect(x, y, width, height), 3, 3);
			}
			
			#endregion
		}
		
		private bool IsOffsetInsideViewport(int offsetFrom, int offsetTo)
		{
			if (offsetFrom < viewportFirstLine.Offset && offsetTo < viewportFirstLine.EndOffset)
				return false;

			if (offsetFrom > viewportLastLine.Offset && offsetTo > viewportLastLine.EndOffset)
				return false;
			
			return true;
		}

		private Point GetPositionFromOffset(VisualYPosition position, int offset)
		{
			var startLocation = editor.TextArea.TextView.Document.GetLocation(offset);
			var point = editor.TextArea.TextView.GetVisualPosition(new TextViewPosition(startLocation), position);

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
