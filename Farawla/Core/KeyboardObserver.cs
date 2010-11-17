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
			
			if (bindings.Count() > 0)
				e.Handled = true;
			
			foreach(var binding in bindings)
				binding.ExecuteCommand(e);
		}
		
		public void AddBinding(KeyboardBinding binding)
		{
			Bindings.Add(binding);
		}

		public void AddBinding(KeyCombination combination, Key key, Action command)
		{
			AddBinding(new KeyboardBinding(combination, key, command));
		}
		
		public void AddBinding(KeyCombination combination, Action<Key> command, params Key[] keys)
		{
			AddBinding(new KeyboardBinding(combination, command, keys));
		}
	}
	
	public class KeyboardBinding
	{
		public Key Key { get; set; }
		public Action Command { get; set; }
		public KeyCombination Combination { get; set; }
		
		public Key[] KeySet { get; set; }
		public Action<Key> ParameterizedCommand { get; set; }

		public KeyboardBinding(KeyCombination combination, Key key, Action command)
		{
			Combination = combination;
			Key = key;
			Command = command;
		}

		public KeyboardBinding(KeyCombination combination, Action<Key> command, params Key[] keys)
		{
			Combination = combination;
			ParameterizedCommand = command;
			KeySet = keys;
		}

		public bool Equals(KeyCombination combination, Key key)
		{
			if (Combination != combination) return false;
			
			if (KeySet == null)
			{
				if (key != Key) return false;
			}
			else
			{
				if (KeySet.Contains(key)) return true;
			}
			

			return true;
		}
		
		public void ExecuteCommand(KeyEventArgs e)
		{
			if (Command != null)
				Command();
			
			if (ParameterizedCommand != null)
				ParameterizedCommand(e.Key);
		}
	}
}