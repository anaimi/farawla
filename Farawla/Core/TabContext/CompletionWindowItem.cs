using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Farawla.Core.TabContext
{
	public enum CompletionItemType
	{
		Object,
		Function,
		Snippet
	}

	public class CompletionWindowItem : ICompletionData
	{
		public string Owner { get; private set; }
		public string Text { get; private set; }
		public object Description { get; private set; }
		public double Priority { get; private set; }

		public object Content { get; private set; }
		public CompletionItemType Type { get; private set; }
		public ImageSource Image { get; private set; }

		public CompletionWindowItem(CompletionItemType type, string owner, string text, string description)
		{
			Owner = owner;
			Text = text;
			Type = type;
			Priority = 0;

			switch (type)
			{
				case CompletionItemType.Object:
					Image = Theme.Instance.ObjectIcon;
					break;

				case CompletionItemType.Function:
					Image = Theme.Instance.FunctionIcon;
					break;

				case CompletionItemType.Snippet:
					Image = Theme.Instance.SnippetIcon;
					break;
			}

			Content = text; // use this property for fancy UI
			
			if (!description.IsBlank())
				Description = description;
		}

		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			var value = Text;

			if (Type == CompletionItemType.Function)
			{
				var index = value.IndexOf('(');

				if (index > 0)
					value = value.Substring(0, index);
			}

			textArea.Document.Replace(completionSegment, value);
		}

		#region Equal & GetHashCode

		public override bool Equals(object obj)
		{
			if (!(obj is CompletionWindowItem))
				return false;

			var other = obj as CompletionWindowItem;

			return other.Owner == Owner && other.Text == Text;
		}

		public override int GetHashCode()
		{
			return (Text + Owner).GetHashCode();
		}

		#endregion
	}
}
