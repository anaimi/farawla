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

namespace Farawla.Features.Terminal
{
	/// <summary>
	/// Interaction logic for TerminalControl.xaml
	/// </summary>
	public partial class TerminalControl : UserControl
	{
		private Inline caretInline;
		
		public TerminalControl()
		{
			InitializeComponent();

			caretInline = Input.Inlines.LastInline;

			RTB.PreviewMouseDown += OnControlMouseDown;
			//TextReceiver.TextChanged += TextChanged;
		}

		private void CursorPositionChanged(object sender, QueryCursorEventArgs e)
		{
			
		}

		private void OnControlMouseDown(object sender, MouseButtonEventArgs e)
		{
			//TextReceiver.BackgroundFocus();

			RTB.Selection.Select(RTB.Document.ContentEnd, RTB.Document.ContentEnd);
		}

		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			var value = new Run(TextReceiver.Text);
			TextReceiver.Text = "";

			Input.Inlines.InsertBefore(caretInline, value);
		}
	}
}
