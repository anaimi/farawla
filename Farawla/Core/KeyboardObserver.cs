using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Farawla.Core
{
	[Flags]
	public enum KeyCombination
	{
		None = 0,
		Ctrl = 1,
		Alt = 2,
		Shift = 4,
	}
	
	public class KeyboardObserver
	{		
		public List<KeyboardBinding> Bindings { get; private set; }

		public KeyboardObserver()
		{
			Bindings = new List<KeyboardBinding>();
			Controller.Current.MainWindow.KeyDown += KeyboardEventOccured;
		}

		private void KeyboardEventOccured(object sender, KeyEventArgs e)
		{
			var combination = KeyCombination.None;
			
			if (Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift))
				combination |= KeyCombination.Shift;
			
			if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
				combination |= KeyCombination.Ctrl;
			
			if (Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))
				combination |= KeyCombination.Alt;

			var bindings = Bindings.Where(b => b.Equals(combination, e.Key));
			
			foreach(var binding in bindings)
				binding.Command();
		}

		public void AddBinding(KeyCombination combination, Key key, Action command)
		{
			Bindings.Add(new KeyboardBinding(combination, key, command));
		}
	}
	
	public class KeyboardBinding
	{
		public Key Key { get; set; }
		public Action Command { get; set; }
		public KeyCombination Combination { get; set; }

		public KeyboardBinding(KeyCombination combination, Key key, Action command)
		{
			Combination = combination;
			Key = key;
			Command = command;
		}

		public bool Equals(KeyCombination combination, Key key)
		{
			if (Combination != combination) return false;
			if (key != Key) return false;

			return true;
		}
	}
}