using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Farawla.Core;
using Farawla.Core.Sidebar;
using Newtonsoft.Json;
using System.Windows.Media;
using Farawla.Core.Language;

namespace Farawla.Features.Completion
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public WidgetSettings Settings { get; set; }
		public Dictionary<string, AutoComplete> LanguageCompletions { get; set; }
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Completion");
			SidebarButton.IsExpandable = true;
			SidebarButton.IsStretchable = true;

			// get settings
			Settings = Core.Settings.Instance.GetWidgetSettings("Projects");
			
			// arrange
			LanguageCompletions = new Dictionary<string, AutoComplete>();
			
			// assign events
			Controller.Current.OnTabCreated += OnNewTab;
			Controller.Current.OnActiveTabChanged += OnActiveTabChanged;
			Controller.Current.OnStart += OnActiveTabChanged;
		}

		private void OnNewTab(WindowTab tab)
		{
			if (tab.Language.IsDefault)
				return;
			
			// get language completion from cache
			if (!LanguageCompletions.ContainsKey(tab.Language.Name))
			{
				var languagePath = Core.Settings.ExecDir + "\\" + tab.Language.Directory + "\\";
				
				if (File.Exists(languagePath + "autocomplete.js"))
				{
					var json = File.ReadAllText(languagePath + "autocomplete.js");
					var completion = JsonConvert.DeserializeObject<AutoComplete>(json);
					
					completion.Initialize(languagePath);
					
					LanguageCompletions.Add(tab.Language.Name, completion);
					completion.LoadFrameworks(GetEnabledFramrworksFromSettings(tab.Language));
				}
				else
				{
					LanguageCompletions.Add(tab.Language.Name, null);
				}
			}
			
			// get from cache
			var languageCompletion = LanguageCompletions[tab.Language.Name];
			
			// stop if has no completion rules
			if (languageCompletion == null)
				return;
			
			// enable it if it should be enabled
			if (!Settings.KeyExists(tab.Language.Name))
				Settings[tab.Language.Name] = "Disable";

			if (Settings[tab.Language.Name] == "Disable")
				return;
			
			EnableCompletion(tab, languageCompletion);
		}
		
		private void OnActiveTabChanged()
		{
			var tab = Controller.Current.ActiveTab;
			var language = GetLanguageCompletion(tab);

			if (language == null)
			{
				ShowNoCompletionSettings();
				return;
			}
			
			ShowCompletionSettings(tab, language);
		}

		private void TextEntering(WindowTab tab, TextCompositionEventArgs e)
		{
			if (tab.AutoCompleteState == null)
				return;
			
			if (e.Text.Length > 0)
			{
				if (!char.IsLetterOrDigit(e.Text[0]))
				{
					var isDelimiterText = tab.AutoCompleteState.LanguageCompletion.ObjectAttributeDelimiters.Any(d => d == e.Text);

					tab.CompletionRequestInsertion(e, isDelimiterText);
				}
			}
		}

		private void TextEntered(WindowTab tab, TextCompositionEventArgs e)
		{
			if (tab.AutoCompleteState == null)
				return;
			
			if (tab.AutoCompleteState.ShowWindow())
			{
				tab.ShowCompletionWindow(tab.AutoCompleteState.GetCompletionWindowOffset(e.Text));
			}
			else
			{
				tab.HideCompletionWindow();
			}

			tab.AutoCompleteState.TextChanged();
		}
		
		private void ShowNoCompletionSettings()
		{
			CompletionSettings.Visibility = Visibility.Collapsed;
			NoCompletionSettings.Visibility = Visibility.Visible;
		}
		
		private void ShowCompletionSettings(WindowTab tab, AutoComplete language)
		{
			var isDisable = Settings[tab.Language.Name] == "Disable";
			var frameworks = GetEnabledFramrworksFromSettings(tab.Language);
			
			NoCompletionSettings.Visibility = Visibility.Collapsed;
			CompletionSettings.Visibility = Visibility.Visible;

			CompletionState.Content = "Enable for " + tab.Language.Name;
			CompletionState.IsChecked = !isDisable;

			FrameworksContainer.IsEnabled = !isDisable;

			FrameworksContainer.Children.Clear();
			
			foreach (var framework in language.Frameworks)
			{
				var content = framework.Name;
				var cb = new CheckBox();

				cb.Content = content;
				cb.Foreground = new SolidColorBrush(Colors.White);
				
				if (frameworks.Contains(content))
				{
					cb.IsChecked = true;
				}
				
				cb.Click += (s, e) => {
					if (cb.IsChecked.Value)
					{
						EnableFramework(content);
					}
					else
					{
						DisableFramework(content);
					}
				};

				FrameworksContainer.Children.Add(cb);
			}
		}
		
		private AutoComplete GetLanguageCompletion(WindowTab tab)
		{
			if (tab.Language.IsDefault || !LanguageCompletions.ContainsKey(tab.Language.Name))
			{
				return null;
			}

			return LanguageCompletions[tab.Language.Name];
		}

		private List<string> GetEnabledFramrworksFromSettings(LanguageMeta language)
		{
			return Settings[language.Name + "Frameworks"].Split(',').Distinct().ToList();
		}

		private void CompletionStateChanged(object sender, RoutedEventArgs e)
		{
			var isEnabled = CompletionState.IsChecked.Value;
			var completion = GetLanguageCompletion(Controller.Current.ActiveTab);

			Settings[Controller.Current.ActiveTab.Language.Name] = isEnabled ? "Enable" : "Disable";
			
			// inform all tabs
			foreach (var tab in Controller.Current.CurrentTabs.Where(t => t.Language == Controller.Current.ActiveTab.Language))
			{
				if (isEnabled)
				{
					EnableCompletion(tab, completion);
				}
				else
				{
					DisableCompletion(tab);
				}
			}
			
			// update frameworks if enabled
			if (isEnabled)
			{
				ReloadLanguageFrameworks(Controller.Current.ActiveTab.Language);
				FrameworksContainer.IsEnabled = true;				
			}
			else
			{
				FrameworksContainer.IsEnabled = false;				
			}
		}
		
		private void EnableCompletion(WindowTab tab, AutoComplete languageCompletion)
		{
			// get window completion
			tab.AutoCompleteState = new AutoCompleteState(tab, languageCompletion);

			// assign event
			if (!tab.WasAssignedAutoCompletionState)
			{
				tab.Editor.TextArea.TextEntered += (s, e) => TextEntered(tab, e);
				tab.Editor.TextArea.TextEntering += (s, e) => TextEntering(tab, e);
			}

			tab.WasAssignedAutoCompletionState = true;

			// initial population
			tab.AutoCompleteState.TextChanged();
		}

		private void DisableCompletion(WindowTab tab)
		{
			// remove it
			tab.AutoCompleteState = null;
		}
		
		private void EnableFramework(string name)
		{
			var language = Controller.Current.ActiveTab.Language;
			var values = GetEnabledFramrworksFromSettings(language).Where(f => f != name).ToList();
			
			values.Add(name);

			Settings[language.Name + "Frameworks"] = string.Join(",", values.ToArray());

			ReloadLanguageFrameworks(language);
		}
		
		private void DisableFramework(string name)
		{
			var language = Controller.Current.ActiveTab.Language;
			var values = GetEnabledFramrworksFromSettings(language).Where(f => f != name).ToList();

			Settings[language.Name + "Frameworks"] = string.Join(",", values.ToArray());

			ReloadLanguageFrameworks(language);
		}

		private void ReloadLanguageFrameworks(LanguageMeta language)
		{
			var frameworks = GetEnabledFramrworksFromSettings(language);

			Controller.Current.ActiveTab.AutoCompleteState.LanguageCompletion.LoadFrameworks(frameworks);
		}
	}
}
