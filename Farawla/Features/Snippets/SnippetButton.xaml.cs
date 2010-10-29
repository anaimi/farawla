using System;
namespace Farawla.Features.Snippets
{
	public partial class SnippetButton
	{
		public SnippetButton(Snippet snippet, Action<Snippet> onClick)
		{
			InitializeComponent();

			SnippetName.Text = snippet.Name;
			Trigger.Text = snippet.Trigger;
			ToolTip = snippet.Body;

			SnippetBtn.Click += (s, e) => onClick(snippet);
		}
	}
}
