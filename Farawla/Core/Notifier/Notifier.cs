﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Farawla.Core;

namespace Farawla.Features
{
	public class Notifier
	{
		public delegate void OnPromptEvent(bool canceled, string msg);
		
		public static void Show(string message)
		{
			MessageBox.Show(message);
		}

		public static void Prompt(string description, string detail, string input, OnPromptEvent action)
		{
			Prompt(description, detail, input, -1, action);
		}		

		public static void Prompt(string description, string detail, string input, int highlight, OnPromptEvent action)
		{
			var prompt = new ModalInputBox(description, detail, input, highlight, action);

			prompt.ShowDialog();
		}
	}
}
