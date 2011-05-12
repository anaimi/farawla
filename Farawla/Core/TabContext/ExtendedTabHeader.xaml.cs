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

namespace Farawla.Core.TabContext
{
	/// <summary>
	/// Interaction logic for ExtendedTabHeader.xaml
	/// </summary>
	public partial class ExtendedTabHeader : UserControl
	{
		public Tab Tab { get; set; }
		public bool IsSelected
		{
			get
			{
				return TabControl.GetIsSelected(Tab.TabItem);
			}
		}
		
		public ExtendedTabHeader(Tab tab)
		{
			InitializeComponent();

			Tab = tab;
			TabCaption.Text = tab.Name;
			
			if (!tab.DocumentPath.IsBlank())
			{
				ToolTip = tab.DocumentPath;
			}

			Controller.Current.MainWindow.Tab.SelectionChanged += ActiveTabChanges; // un-listen when closed
			CloseBtn.Click += CloseBtnClicked;
		}

		private void CloseBtnClicked(object sender, RoutedEventArgs e)
		{
			if (!Settings.Instance.ShowCloseButtonInTabHeader)
				return;
			
			// un assign
			Controller.Current.MainWindow.Tab.SelectionChanged -= ActiveTabChanges;
			
			Tab.Close();
		}

		private void ActiveTabChanges(object sender, SelectionChangedEventArgs e)
		{
			if (Controller.Current.ActiveTab == Tab)
			{
				Left.Fill = ThemeColorConverter.GetColor("WindowTabSelectedColor");
				Middle.Fill = ThemeColorConverter.GetColor("WindowTabSelectedColor");
				Right.Fill = ThemeColorConverter.GetColor("WindowTabSelectedColor");
			}
			else
			{
				Left.Fill = ThemeColorConverter.GetColor("WindowTabInactiveColor");
				Middle.Fill = ThemeColorConverter.GetColor("WindowTabInactiveColor");
				Right.Fill = ThemeColorConverter.GetColor("WindowTabInactiveColor");
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			if (Settings.Instance.ShowCloseButtonInTabHeader)
				CloseBtn.Opacity = 0.6;

			if (!IsSelected)
			{
				Left.Fill = ThemeColorConverter.GetColor("WindowTabHoverColor");
				Middle.Fill = ThemeColorConverter.GetColor("WindowTabHoverColor");
				Right.Fill = ThemeColorConverter.GetColor("WindowTabHoverColor");
			}
			
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			CloseBtn.Opacity = 0;

			if (!IsSelected)
			{
				Left.Fill = ThemeColorConverter.GetColor("WindowTabInactiveColor");
				Middle.Fill = ThemeColorConverter.GetColor("WindowTabInactiveColor");
				Right.Fill = ThemeColorConverter.GetColor("WindowTabInactiveColor");
			}
			
			base.OnMouseLeave(e);
		}

		public void MarkAsUnSaved()
		{
			TabCaption.Text = Tab.Name + "*";
		}

		public void MarkAsSaved()
		{
			TabCaption.Text = Tab.Name;
		}
	}
}
