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
using ICSharpCode.AvalonEdit.Rendering;

namespace Farawla.Core.TabContext
{
	/// <summary>
	/// Interaction logic for ExtendedTabItem.xaml
	/// </summary>
	public partial class ExtendedTabItem : TabItem
	{
		private Tab tab;
		
		public ExtendedTabItem(Tab tab)
		{
			this.tab = tab;
			
			InitializeComponent();

			Tag = tab;
			Header = tab.TabHeader;
			Content = tab.Editor;
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			if (!(e.OriginalSource is TextView))
				       tab.Close();
			
			base.OnMouseDoubleClick(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			e.Handled = true;
			
			base.OnMouseLeftButtonDown(e);
		}
	}
}
