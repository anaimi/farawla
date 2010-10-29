using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Farawla.Core.Sidebar;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Newtonsoft.Json;

namespace Farawla.Features.Completion
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public Dictionary<string, AutoComplete> Completions { get; set; }
		
		private BackgroundWorker completionWorker;
		private List<CompletionWindowItem> completionItems;
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Completion");
			SidebarButton.IsExpandable = true;
			SidebarButton.WidgetHeight = 140;
			
			// arrange
			Completions = new Dictionary<string, AutoComplete>();
			
			// assign events
			Controller.Current.OnTabCreated += OnNewTab;
			Controller.Current.OnStart += () => Controller.Current.CurrentTabs.ForEach(OnNewTab);
		}

		private void OnNewTab(WindowTab tab)
		{
			if (tab.Language.IsDefault)
				return;
			
			// make sure it exists in the cache
			if (!Completions.ContainsKey(tab.Language.Name))
			{
				if (File.Exists(Core.Settings.ExecDir + "\\" + tab.Language.Directory + "\\autocomplete.js"))
				{
					var json = File.ReadAllText(Core.Settings.ExecDir + "\\" + tab.Language.Directory + "\\autocomplete.js");
					
					Completions.Add(tab.Language.Name, JsonConvert.DeserializeObject<AutoComplete>(json));
				}
				else
				{
					Completions.Add(tab.Language.Name, null);
				}
			}
			
			// get from cache
			var completion = Completions[tab.Language.Name];
			
			// stop if has no completion rules
			if (completion == null)
				return;

			// assign event
			tab.Editor.TextArea.TextEntered += (s, e) => TextEntered(tab, completion, e);
			
			// initialize completion worker and window
			completionWorker = new BackgroundWorker();
			completionWorker.DoWork += (s, e) => PopulateAutoComplete(e);
		}

		private void TextEntered(WindowTab tab, AutoComplete completion, TextCompositionEventArgs args)
		{
			if (completionItems != null && args.Text.Length == 1 && args.Text[0] == '.')
			{
				var cw = new CompletionWindow(tab.Editor.TextArea);

				foreach (var item in completionItems)
					cw.CompletionList.CompletionData.Add(item);

				Debug.WriteLine("Refreshed completion list: " + completionItems.Count + " items.");

				cw.Show();
				cw.Closed += (s, e) => cw = null;
			}
			else if (!completionWorker.IsBusy)
			{
				completionWorker.RunWorkerAsync(new EditorState { Text = tab.Editor.Text, CaretOffset = tab.Editor.CaretOffset, Completion = completion });
			}
		}

		public void PopulateAutoComplete(DoWorkEventArgs args)
		{
			var state = args.Argument as EditorState;

			var identifiers = state.Completion.GetIdentifiersFromCode(state.Text).Where(i => i.Scope.From < state.CaretOffset && i.Scope.To >= state.CaretOffset);
			completionItems = new List<CompletionWindowItem>();

			foreach (var identifier in identifiers)
			{
				completionItems.Add(new CompletionWindowItem(identifier.Name));
			}
		}

		internal class EditorState
		{
			public AutoComplete Completion { get; set; }
			public string Text { get; set; }
			public int CaretOffset { get; set; }
		}
	}
}
