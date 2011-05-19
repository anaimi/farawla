using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Farawla.Core;

namespace Farawla.Features.Snippets
{
	/// <summary>
	/// Interaction logic for SnippetTooptip.xaml
	/// </summary>
	public partial class SnippetTooltip : UserControl
	{
		public Theme Theme { get; set; }
		
		public SnippetTooltip(Snippet snippet)
		{
			InitializeComponent();
			
			Code.Text = snippet.Body;
			Trigger.Text = snippet.Trigger;

			Theme = Theme.Instance;
		}
	}
}
