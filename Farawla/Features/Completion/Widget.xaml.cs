using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Farawla.Core;
using Farawla.Core.Sidebar;
using Farawla.Core.TabContext;

namespace Farawla.Features.Completion
{
	public partial class Widget : IWidget
	{
		public BarButton SidebarButton { get; set; }
		public WidgetSettings Settings { get; set; }
		
		public AutoComplete ActiveCompletion { get; set; }
		public CompletionEngine Engine { get; set; }
		public Tab ActiveTab
		{
			get { return Controller.Current.ActiveTab; }
		}
		
		public Widget()
		{
			InitializeComponent();

			// create sidebar button
			SidebarButton = new BarButton(this, "Completion");
			SidebarButton.IsExpandable = true;
			SidebarButton.IsStretchable = true;

			// get settings
			Settings = Core.Settings.Instance.GetWidgetSettings("Completion");
			
			// assign events
			Controller.Current.OnTabCreated += OnNewTab;
			Controller.Current.OnContextLanguageChanged += OnContextLanguageChanged;
			
			// keyboard binding
			Controller.Current.Keyboard.AddBinding(KeyCombination.Ctrl, Key.Space, ManuallyShowCompletionWindow);
			
			// arrange
			Engine = new CompletionEngine();
		}

		private void OnNewTab(Tab tab)
		{
			// always listen to caret move
			tab.Editor.TextArea.TextEntered += TextEntered;
			tab.Editor.TextArea.TextEntering += TextEntering;
		}

		private void OnContextLanguageChanged(EditorSegment segment)
		{
			var completion = AutoComplete.GetCompletion(segment.SyntaxName);
			
			Engine.Reset(segment, completion);

			if (completion == null)
			{
				ShowNoCompletionSettings();
			}
			else
			{
				ShowCompletionSettings(completion, segment.SyntaxName);
			}
		}

		private void TextEntering(object sender, TextCompositionEventArgs e)
		{
			if (!Engine.IsEnabled)
				return;

			if (e.Text.Length == 1)
			{
				if (!Engine.IsIdentifierCharacter(e.Text[0]))
				{
					var isDelimiterText = ActiveCompletion.ObjectAttributeDelimiters.Any(d => d == e.Text);

					ActiveTab.CompletionRequestInsertion(e, isDelimiterText);
				}
			}
		}

		private void TextEntered(object sender, TextCompositionEventArgs e)
		{
			if (!Engine.IsEnabled)
				return;

			if (Engine.CanShowWindow())
			{
				var isDelimiter = ActiveCompletion.ObjectAttributeDelimiters.Any(d => d == e.Text);

				ActiveTab.ShowCompletionWindow(GetCompletionWindowOffset(e.Text), e.Text, isDelimiter);
			}
			else
			{
				ActiveTab.HideCompletionWindow();
			}

			Engine.TextChanged();
		}

		private void ManuallyShowCompletionWindow()
		{
			if (!Engine.IsEnabled)
				return;

			if (Engine.IsInIgnoredSection())
				return;

			var enteredText = string.Empty;
			Engine.PopulateTokensBeforeCaret();

			if (Engine.TokensBeforeCaret.Count > 0)
				enteredText = Engine.TokensBeforeCaret[0];

			ActiveTab.ShowCompletionWindow(GetCompletionWindowOffset(""), enteredText, false);
		}		

		public int GetCompletionWindowOffset(string enteredText)
		{
			if (ActiveCompletion.ObjectAttributeDelimiters.Contains(enteredText))
				return ActiveTab.Editor.CaretOffset;

			if (Engine.TokensBeforeCaret.Count > 0)
				return ActiveTab.Editor.CaretOffset - Engine.TokensBeforeCaret.First().Length;

			return ActiveTab.Editor.CaretOffset - enteredText.Length;
		}
		
		#region UI clicks handlers (enable, disable)

		private void CompletionStateChanged(object sender, RoutedEventArgs e)
		{
			var isEnabled = CompletionState.IsChecked.Value;
			
			Settings[ActiveCompletion.LanguageName] = isEnabled ? "Enable" : "Disable";

			// update frameworks if enabled
			if (isEnabled)
			{
				ReloadLanguageFrameworks();
				FrameworksContainer.IsEnabled = true;
			}
			else
			{
				FrameworksContainer.IsEnabled = false;
			}
		}

		private void EnableFramework(string frameworkName)
		{
			var frameworks = ActiveCompletion.GetEnabledFrameworks().Where(f => f != frameworkName).ToList();

			frameworks.Add(frameworkName);

			Settings[ActiveCompletion.LanguageName + "Frameworks"] = string.Join(",", frameworks.ToArray());
			ReloadLanguageFrameworks();
		}

		private void DisableFramework(string frameworkName)
		{
			var values = ActiveCompletion.GetEnabledFrameworks().Where(f => f != frameworkName).ToList();

			Settings[ActiveCompletion.LanguageName + "Frameworks"] = string.Join(",", values.ToArray());
			ReloadLanguageFrameworks();
		}

		private void ReloadLanguageFrameworks()
		{
			var frameworks = ActiveCompletion.GetEnabledFrameworks();
			
			ActiveCompletion.LoadFrameworks(frameworks);
		}		

		#endregion

		private void ShowCompletionSettings(AutoComplete completion, string languageName)
		{
			var language = Controller.Current.Languages.GetLanguageByName(languageName);
			var isDisable = Settings[language.Name] == "Disable";
			var frameworks = completion.GetEnabledFrameworks();

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

		private bool IsLanguageEnabled(string languageName)
		{
			if (!Settings.KeyExists(languageName))
				Settings[languageName] = "Disable";

			if (Settings[languageName] == "Disable")
				return false;

			return true;
		}
		
	}
}
