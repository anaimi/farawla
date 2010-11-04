using System.Collections.Generic;
using System.IO;
using Farawla.Core;
using Farawla.Core.Sidebar;
using Newtonsoft.Json;

namespace Farawla.Features.Completion
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public Dictionary<string, AutoComplete> LanguageCompletions { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Completion");
			SidebarButton.IsExpandable = true;
			SidebarButton.WidgetHeight = 140;
			
			// arrange
			LanguageCompletions = new Dictionary<string, AutoComplete>();
			
			// assign events
			Controller.Current.OnTabCreated += OnNewTab;
		}

		private void OnNewTab(WindowTab tab)
		{
			if (tab.Language.IsDefault)
				return;
			
			// get language completion from cache
			if (!LanguageCompletions.ContainsKey(tab.Language.Name))
			{
				if (File.Exists(Core.Settings.ExecDir + "\\" + tab.Language.Directory + "\\autocomplete.js"))
				{
					var json = File.ReadAllText(Core.Settings.ExecDir + "\\" + tab.Language.Directory + "\\autocomplete.js");

					LanguageCompletions.Add(tab.Language.Name, JsonConvert.DeserializeObject<AutoComplete>(json));
				}
				else
				{
					LanguageCompletions.Add(tab.Language.Name, null);
				}
			}
			
			// get from cache
			var lanuageCompletion = LanguageCompletions[tab.Language.Name];
			
			// stop if has no completion rules
			if (LanguageCompletions == null)
				return;
			
			// get window completion
			var windowCompleton = new AutoCompleteState(tab, lanuageCompletion);

			// assign event
			tab.Editor.TextArea.TextEntered += (s, e) => TextEntered(windowCompleton);
			
			// initial population
			windowCompleton.TextChanged();
		}

		private void TextEntered(AutoCompleteState windowCompletion)
		{
			if (windowCompletion.ShowWindow())
			{
				windowCompletion.Tab.ShowCompletionWindow();
			}
			else
			{
				windowCompletion.Tab.HideCompletionWindow();
			}

			windowCompletion.TextChanged();
		}
	}
}
