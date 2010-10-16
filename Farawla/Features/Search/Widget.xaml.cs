using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using Farawla.Core.Sidebar;
using Farawla.Core;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace Farawla.Features.Search
{
	public partial class Widget : IWidget
	{
		public enum SearchAction
		{
			Highlight,
			Jump
		}
		
		public BarButton SidebarButton { get; set; }

		private int lastReachedOffset;
		private Color foundTextBackground;
		
		public Widget()
		{
			InitializeComponent();
			
			// create sidebar button
			SidebarButton = new BarButton(this, "Search");
			SidebarButton.IsExpandable = true;
			SidebarButton.WidgetHeight = 140;
			
			// add keyboard shortcut
			Controller.Current.Keyboard.AddBinding(KeyCombination.None, Key.F3, ShowWidgetAndGetFocus);
			Controller.Current.Keyboard.AddBinding(KeyCombination.Ctrl, Key.F, ShowWidgetAndGetFocus);
			
			// set default color
			foundTextBackground = Colors.Yellow;
			
			// set events
			Query.KeyUp += OnQueryKeyUp;
			Query.LostFocus += OnQueryLostFocus;
		}

		private void OnQueryLostFocus(object sender, RoutedEventArgs e)
		{
			ClearAllHighlightedBlocks();
		}

		private void ClearAllHighlightedBlocks()
		{
			foreach (var tab in Controller.Current.CurrentTabs)
			{
				tab.BlockHighlighter.Clear(this);
				tab.BlockHighlighter.Redraw();
			}
		}

		private void OnQueryKeyUp(object sender, KeyEventArgs e)
		{
			if (Query.Text.IsBlank())
			{
				ClearAllHighlightedBlocks();
				return;
			}
			
			if (e.Key == Key.Enter)
			{
				DoSearch(SearchAction.Jump);
			}
			else
			{
				DoSearch(SearchAction.Highlight);
			}
		}

		private void ShowWidgetAndGetFocus()
		{
			SidebarButton.ExpandWidget();

			Query.Focus();
		}

		private void DoSearch(SearchAction action)
		{
			var expression = Regex.Escape(Query.Text);
			var regex = new Regex(expression);

			foreach (var tab in Controller.Current.CurrentTabs)
			{
				DoSearch(action, tab, regex);
			}
		}

		private void DoSearch(SearchAction action, WindowTab tab, Regex regex)
		{
			// clear old blocks
			tab.BlockHighlighter.Clear(this);
			
			// highlight matches
			if (action == SearchAction.Highlight)
			{
				lastReachedOffset = 0;
				
				foreach (Match match in regex.Matches(tab.Editor.Text))
				{
					tab.BlockHighlighter.Add(this, match.Index, match.Length, foundTextBackground);
				}
			}
			
			// job to matches
			else if (action == SearchAction.Jump)
			{
				// search
				var match = regex.Match(tab.Editor.Text, lastReachedOffset);
				
				// end of document? restart
				if (!match.Success && lastReachedOffset > 0)
				{
					lastReachedOffset = 0;
					match = regex.Match(tab.Editor.Text, lastReachedOffset);
				}
				
				// update offset
				lastReachedOffset = match.Index + match.Length;

				// select
				tab.Editor.Select(match.Index, match.Length);
				
				// if not active, make it active
				tab.MakeActive();
				
				// scroll
				var line = tab.Editor.Document.GetLineByOffset(match.Index).LineNumber;
				tab.Editor.ScrollToLine(line);
			}

			// redraw
			tab.BlockHighlighter.Redraw();
		}

		private void FindClicked(object sender, RoutedEventArgs e)
		{
			DoSearch(SearchAction.Jump);
		}

		private void ReplaceClicked(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
