using System;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Farawla.Features.Snippets
{
	public partial class SnippetButton
	{
		public SnippetButton(Snippet snippet, Action<Snippet> onClick)
		{
			InitializeComponent();

			SnippetName.Text = snippet.Name;
			Trigger.Text = snippet.Trigger;
			
			ToolTip = new SnippetTooltip(snippet);
			ToolTipService.SetPlacement(this, PlacementMode.Left);

			SnippetBtn.Click += (s, e) => onClick(snippet);
		}
	}
}
