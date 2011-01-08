using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Farawla.Core.Sidebar;
using Farawla.Core;
using System.Windows.Input;
using System.Windows.Media;
using Farawla.Core.TabContext;

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

		private Tab lastReachedTab;
		private int lastReachedOffset;
		private Color foundTextBackground;
		
		public Widget()
		{
			InitializeComponent();
			
			// create sidebar button
			SidebarButton = new BarButton(this, "Search");
			SidebarButton.IsExpandable = true;
			SidebarButton.WidgetHeight = 190;
			
			// add keyboard shortcut
			Controller.Current.Keyboard.AddBinding(KeyCombination.Ctrl, Key.F, ShowWidgetAndGetFocus);
			Controller.Current.Keyboard.AddBinding(KeyCombination.None, Key.F3, () => {
				if (!Query.Text.IsBlank())
					DoSearch(SearchAction.Jump);
				else
					ShowWidgetAndGetFocus();
            });
			
			// set default color
			foundTextBackground = Theme.Instance.MatchingTokensBackground.ToColor();
			
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
				tab.BlockHighlighter.Clear("Search");
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
			Controller.Current.MainWindow.Sidebar.DontHideSidebar = true;
			
			Controller.Current.MainWindow.Sidebar.Visibility = Visibility.Visible;
			Controller.Current.MainWindow.Sidebar.Opacity = 1;
			
			SidebarButton.ExpandWidget(() => {
				Query.Focus();
			});
		}

		private void DoSearch(SearchAction action)
		{
			var regex = GetRegexFromQuery();
			
			if (SearchAreaCurrentDocument.IsSelected)
			{
				DoSearch(action, Controller.Current.ActiveTab, regex);
			}
			else if (SearchAreaOpenDocuments.IsSelected)
			{
				if (action == SearchAction.Highlight)
				{
					DoSearch(action, Controller.Current.ActiveTab, regex);
					return;
				}
				
				if (lastReachedTab == null)
				{
					lastReachedOffset = 0;
					lastReachedTab = Controller.Current.CurrentTabs.First();
				}
				
				DoSearch(action, lastReachedTab, regex);
				
			}
		}

		private void DoSearch(SearchAction action, Tab tab, Regex regex)
		{
			// clear old blocks
			tab.BlockHighlighter.Clear("Search");
			
			// highlight matches
			if (action == SearchAction.Highlight)
			{
				lastReachedOffset = 0;
				
				foreach (Match match in regex.Matches(tab.Editor.Text))
				{
					tab.BlockHighlighter.Add("Search", match.Index, match.Length, foundTextBackground);
				}
			}
			
			// matches
			else if (action == SearchAction.Jump)
			{
				// search
				var match = regex.Match(tab.Editor.Text, lastReachedOffset);
				
				// end of document?
				if (!match.Success)
				{
					NoResultsFound(action, tab, regex);
					return;
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

		private void DoReplace()
		{
			var regex = GetRegexFromQuery();
			var replacement = ReplaceWith.Text;
			
			if (ExtendQuery.IsChecked.Value)
				replacement = GetExtendedString(replacement);
			
			if (SearchAreaCurrentDocument.IsSelected)
			{
				DoReplace(Controller.Current.ActiveTab, regex, replacement);
			}
			else if (SearchAreaOpenDocuments.IsSelected)
			{
				foreach (var tab in Controller.Current.CurrentTabs)
				{
					DoReplace(tab, regex, replacement);
				}
			}
		}

		private void DoReplace(Tab tab, Regex regex, string replacement)
		{
			var offset = 0;
			
			tab.Editor.Document.BeginUpdate();
			
			foreach(Match match in regex.Matches(tab.Editor.Text))
			{
				tab.Editor.Document.Replace(match.Index + offset, match.Length, replacement);
				offset += replacement.Length - match.Length;
			}
			
			tab.Editor.Document.EndUpdate();

			tab.TextChanged();
		}
		
		private Regex GetRegexFromQuery()
		{
			var query = Query.Text;
			
			if (ExtendQuery.IsChecked.Value)
				query = GetExtendedString(query);

			return new Regex(Regex.Escape(query));
		}
		
		private string GetExtendedString(string str)
		{
			return str
					.Replace("\\t", "\t")
					.Replace("\\r", "\r")
					.Replace("\\n", "\n");
		}
		
		private void NoResultsFound(SearchAction action, Tab tab, Regex regex)
		{
			if (SearchAreaCurrentDocument.IsSelected)
			{
				if (lastReachedOffset == 0)
					Notifier.Show("Zero matches");
				else
				{
					lastReachedOffset = 0;
					Notifier.Show("End of document reached...");
				}
			}
			else if (SearchAreaOpenDocuments.IsSelected)
			{
				if (tab == Controller.Current.CurrentTabs.Last())
				{
					Notifier.Show("End of documents reached...");

					lastReachedOffset = 0;
					lastReachedTab = null;
				}
				else
				{
					lastReachedOffset = 0;
					lastReachedTab = Controller.Current.CurrentTabs[tab.Index + 1];
					
					DoSearch(action, lastReachedTab, regex);
				}
			}
		}

		private void FindClicked(object sender, RoutedEventArgs e)
		{
			DoSearch(SearchAction.Jump);
		}

		private void ReplaceClicked(object sender, RoutedEventArgs e)
		{
			DoReplace();
		}
	}
}
