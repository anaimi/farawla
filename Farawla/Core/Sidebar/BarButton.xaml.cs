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

namespace Farawla.Core.Sidebar
{
	/// <summary>
	/// Interaction logic for SidebarButton.xaml
	/// </summary>
	public partial class BarButton : UserControl
	{
		public BarButton()
		{
			InitializeComponent();
		}

		public BarButton(string label, Action onClick)
		{
			InitializeComponent();
			
			SetLabel(label);
			
			MouseLeftButtonDown += (s, e) => {
				if (onClick != null)
					onClick();
			};
		}
		
		public void SetLabel(string label)
		{
			Label.Text = label;
		}
	}
}
