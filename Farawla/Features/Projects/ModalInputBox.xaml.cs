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
using System.Windows.Shapes;

namespace Farawla.Features.Projects
{
	/// <summary>
	/// Interaction logic for ModalInputBox.xaml
	/// </summary>
	public partial class ModalInputBox : Window
	{
		public delegate void OnEnterEvent(bool canceled, string input);

		private OnEnterEvent OnEnter { get; set; }

		public ModalInputBox(string title, string primaryDescription, string secondaryDescription, string input, OnEnterEvent onEnter)
		{
			InitializeComponent();

			Title = title;
			DescriptionPrimary.Text = primaryDescription;
			DescriptionSecondary.Text = secondaryDescription;
			Input.Text = input;
			OnEnter = onEnter;

			Input.KeyDown += InputKeyDown;
			
			if (Input.Text.Length > 0)
				Input.Select(0, Input.Text.Length);

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
