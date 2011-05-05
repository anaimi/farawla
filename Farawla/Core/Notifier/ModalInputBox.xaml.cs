using System.Windows;
using System.Windows.Input;
using Farawla.Features;

namespace Farawla.Core
{
	public partial class ModalInputBox : Window
	{
		private Notifier.OnPromptEvent OnEnter { get; set; }

		public ModalInputBox(string description, string details, string input, int highlight, Notifier.OnPromptEvent onEnter)
		{
			InitializeComponent();

			DescriptionPrimary.Text = description;
			DescriptionSecondary.Text = details;
			Input.Text = input;
			OnEnter = onEnter;

			Input.KeyDown += InputKeyDown;

			if (Input.Text.Length > 0)
			{
				if (highlight == -1)
				{
					Input.Select(0, Input.Text.Length);
				}
				else if (highlight > 0 && Input.Text.Length >= highlight)
				{
					Input.Select(0, highlight);
				}
			}

			Loaded += (s, e) => Input.Focus();
		}

		private void InputKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (OnEnter != null)
					OnEnter(false, Input.Text);
				
				Close();
			}
			else if (e.Key == Key.Escape)
			{
				if (OnEnter != null)
					OnEnter(true, Input.Text);

				Close();
			}
		}
	}
}