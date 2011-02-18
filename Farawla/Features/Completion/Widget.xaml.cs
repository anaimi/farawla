using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Farawla.Core;
using Farawla.Core.Sidebar;
using Farawla.Core.TabContext;
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
		public AutoComplete ActiveCompletion { get; set; }
		
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
			Controller.Current.OnContextLanguageChanged += OnContextLanguageChanged;
			
			// keyboard binding
			Controller.Current.Keyboard.AddBinding(KeyCombination.Ctrl, Key.Space, ManuallyShowCompletionWindow);
		}

		private void ManuallyShowCompletionWindow()
		{
			var tab = Controller.Current.ActiveTab;
			
			if (tab.AutoCompleteState == null)
				return;
			
			if (tab.AutoCompleteState.IsInIgnoredSection())
				return;

			var enteredText = string.Empty;
			tab.AutoCompleteState.PopulateTokensBeforeCaret();

			if (tab.AutoCompleteState.TokensBeforeCaret.Count > 0)
				enteredText = tab.AutoCompleteState.TokensBeforeCaret[0];

			tab.ShowCompletionWindow(tab.AutoCompleteState.GetCompletionWindowOffset(""), enteredText, false);
		}

		private void OnNewTab(Tab tab)
		{
			if (tab.Language.IsDefault)
				return;
			
			// always listen to caret move
			tab.Editor.TextArea.TextEntered += TextEntered;
			tab.Editor.TextArea.TextEntering += TextEntering;
			
			// get from cache
			var languageCompletion = GetLanguageCompletion(tab.Language.Name);
			
			// stop if has no completion rules
			if (languageCompletion == null)
				return;
			
			// enable it if it should be enabled
			if (IsEnabledLanguage(tab.Language.Name))
			{
				EnableCompletion(tab, languageCompletion);
			}
		}

		private void OnContextLanguageChanged(string languageName)
		{
			AttemptToShowCompletionSettings(languageName);
		}

		private void TextEntering(object sender, TextCompositionEventArgs e)
		{
			if (ActiveCompletion == null)
				return;

			var tab = Controller.Current.ActiveTab;
			
			if (e.Text.Length == 1)
			{
				if (!tab.AutoCompleteState.IsIdentifierCharacter(e.Text[0]))
				{
					var isDelimiterText = tab.AutoCompleteState.LanguageCompletion.ObjectAttributeDelimiters.Any(d => d == e.Text);

					tab.CompletionRequestInsertion(e, isDelimiterText);
				}
			}
		}

		private void TextEntered(object sender, TextCompositionEventArgs e)
		{
			if (ActiveCompletion == null)
				return;

			var tab = Controller.Current.ActiveTab;
			
			if (tab.AutoCompleteState.ShowWindow())
			{
				var isDelimiter = tab.AutoCompleteState.LanguageCompletion.ObjectAttributeDelimiters.Any(d => d == e.Text);

				tab.ShowCompletionWindow(tab.AutoCompleteState.GetCompletionWindowOffset(e.Text), e.Text, isDelimiter);
			}
			else
			{
				tab.HideCompletionWindow();
			}

			tab.AutoCompleteState.TextChanged();
		}
		
		
		private void AttemptToShowCompletionSettings(string languageName)
		{
			var language = GetLanguageCompletion(languageName);

			if (language == null)
			{
				ShowNoCompletionSettings();
				return;
			}

			ShowCompletionSettings(languageName, language);
		}

		
		private AutoComplete GetLanguageCompletion(string languageName)
		{
			if (languageName == LanguageMeta.DEFAULT_NAME)
				return null;
			
			if (!LanguageCompletions.ContainsKey(languageName))
			{
				var language = Controller.Current.Languages.GetLanguageByName(languageName);
				var languagePath = language.Directory + "\\";

				if (File.Exists(languagePath + "autocomplete.js"))
				{
					var json = File.ReadAllText(languagePath + "autocomplete.js");
					var completion = JsonConvert.DeserializeObject<AutoComplete>(json);

					completion.Initialize(languagePath);

					LanguageCompletions.Add(language.Name, completion);
					completion.LoadFrameworks(GetEnabledFramrworksFromSettings(language));
				}
				else
				{
					LanguageCompletions.Add(language.Name, null);
				}
			}

			if (LanguageCompletions[languageName] == null)
				return null;

			return LanguageCompletions[languageName];
		}

		private List<string> GetEnabledFramrworksFromSettings(LanguageMeta language)
		{
			return Settings[language.Name + "Frameworks"].Split(',').Distinct().ToList();
		}
		
		private void EnableCompletion(Tab tab, AutoComplete languageCompletion)
		{
			// get window completion
			tab.AutoCompleteState = new AutoCompleteState(tab, languageCompletion);

			// initial population
			tab.AutoCompleteState.TextChanged();
		}

		private bool IsEnabledLanguage(string languageName)
		{
			if (!Settings.KeyExists(languageName))
				Settings[languageName] = "Disable";

			if (Settings[languageName] == "Disable")
				return false;

			return true;
		}

		#region UI clicks handlers (enable, disable)

		private void CompletionStateChanged(object sender, RoutedEventArgs e)
		{
			var isEnabled = CompletionState.IsChecked.Value;
			var completion = GetLanguageCompletion(Controller.Current.ActiveTab.Language.Name);

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

		private void DisableCompletion(Tab tab)
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

		#endregion

		private void ShowCompletionSettings(string languageName, AutoComplete completion)
		{
			var language = Controller.Current.Languages.GetLanguageByName(languageName);
			var isDisable = Settings[language.Name] == "Disable";
			var frameworks = GetEnabledFramrworksFromSettings(language);

			ActiveCompletion = completion;
			NoCompletionSettings.Visibility = Visibility.Collapsed;
			CompletionSettings.Visibility = Visibility.Visible;

			CompletionState.Content = "Enable for " + language.Name;
			CompletionState.IsChecked = !isDisable;

			FrameworksContainer.IsEnabled = !isDisable;

			FrameworksContainer.Children.Clear();

			foreach (var framework in ActiveCompletion.Frameworks)
			{
				var content = framework.Name;
				var cb = new CheckBox();

				cb.Content = content;

				if (frameworks.Contains(content))
				{
					cb.IsChecked = true;
				}

				cb.Click += (s, e) =>
				{
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

		private void ShowNoCompletionSettings()
		{
			ActiveCompletion = null;
			CompletionSettings.Visibility = Visibility.Collapsed;
			NoCompletionSettings.Visibility = Visibility.Visible;
		}
		
	}
}
