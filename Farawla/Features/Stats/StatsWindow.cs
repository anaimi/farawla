using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Farawla.Core.Sidebar;
using Farawla.Core.TabContext;
using Farawla.Features;
using Farawla.Core;
using System.ComponentModel;
using System.IO;

namespace Farawla.Features.Stats
{
	/// <summary>
	/// Interaction logic for NotifyWindow.xaml
	/// </summary>
	public partial class StatsWindow : IWidget
	{
		public BarButton SidebarButton { get; set; }

		private double sidebarOpacity;
		
		public StatsWindow()
		{
			InitializeComponent();
						
			// create sidebar button
			SidebarButton = new BarButton(this, "Stats Window") { ShowInSidebar = false };		
			
			// bind events
			Controller.Current.MainWindow.KeyDown += OnKeyDown;
			KeyUp += OnKeyUp;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (!Keyboard.IsKeyDown(Key.RightAlt) && !Keyboard.IsKeyDown(Key.LeftAlt))
				return;

			if (!Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftCtrl))
				return;

			var rect = Controller.Current.MainWindow.Sidebar.TransformToVisual(Controller.Current.MainWindow).TransformBounds(LayoutInformation.GetLayoutSlot(Controller.Current.MainWindow.Sidebar.Sidebar));

			Top = rect.Top + Controller.Current.MainWindow.Sidebar.Margin.Top + Controller.Current.MainWindow.Top;
			Left = rect.Left - Width - 20 + Controller.Current.MainWindow.Left;
			
			// get and set sidebar opacity
			Controller.Current.MainWindow.Sidebar.DontHideSidebar = true;
			sidebarOpacity = Controller.Current.MainWindow.Sidebar.Opacity;
			Controller.Current.MainWindow.Sidebar.Opacity = 1;
			
			// get stats
			ResetStats();
			PopulateStats();

			// show
			ShowDialog();
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			HideStats();
		}
		
		private void HideStats()
		{
			// set sidebar opacity
			Controller.Current.MainWindow.Sidebar.Opacity = sidebarOpacity;
			Controller.Current.MainWindow.Sidebar.DontHideSidebar = false;

			// hide
			Hide();
		}
		
		private void ResetStats()
		{
			Position.Text = "?";
			Words.Text = "?";
			Chars.Text = "?";
			LastModified.Text = "?";
			FileSize.Text = "?";
		}
		
		private void PopulateStats()
		{
			var tab = Controller.Current.ActiveTab;
			
			// position
			var location = tab.Editor.Document.GetLocation(tab.Editor.CaretOffset);
			Position.Text = "Line " + location.Line + ", Column " + location.Column + ", Offset " + tab.Editor.CaretOffset;

			// words
			var wordsCount = Regex.Matches(tab.Editor.Text, @"[\S]+").Count;
			Words.Text = wordsCount.Pluralize("word", "words");

			// chars
			var charCount = tab.Editor.Text.Length;
			Chars.Text = charCount.Pluralize("character", "characters");

			// date created
			if (tab.IsNewDocument)
			{
				DateCreated.Text = "never";
			}
			else
			{
				DateCreated.Text = File.GetCreationTime(tab.DocumentPath).ToString("MMM dd, yyyy @ hh:mm tt");
			}

			
			if (tab.IsNewDocument)
			{
				// last modified
				LastModified.Text = "never";
				
				// encoding
				Encoding.Text = "not set";
				
				// file size
				FileSize.Text = "unknown";
			}
			else
			{
				// last modified
				LastModified.Text = File.GetLastWriteTime(tab.DocumentPath).ToString("MMM dd, yyyy @ hh:mm tt");

				// encoding
				Encoding.Text = tab.Editor.Encoding.EncodingName;

				// file size
				FileSize.Text = tab.Editor.Encoding.GetByteCount(tab.Editor.Text).ToPrettyBytes();
			}
		}

		private void EncodingBtnClick(object sender, RoutedEventArgs e)
		{
			EncodingBtn.ContextMenu.IsOpen = true;
		}

		private void EncodingChangeSelected(object sender, RoutedEventArgs e)
		{
			var item = sender as MenuItem;

			Controller.Current.ActiveTab.Editor.Encoding = System.Text.Encoding.GetEncoding(item.Tag as string);
			
			HideStats();
		}
	}
}