﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Farawla.Core.TabContext;

namespace Farawla.Features.Completion
{
	public class AutoCompleteState
	{
		public Tab Tab { get; set; }
		public AutoComplete LanguageCompletion { get; set; }

		public List<IdentifierMatch> GlobalIdentifiers { get; set; }
		public List<AutoCompleteItem> AvailableOptions { get; set; }
		public List<ScopeRange> IgnoredScopes { get; set; }
		public List<string> TokensBeforeCaret { get; set; }

		private BackgroundWorker completionWorker;

		public AutoCompleteState(Tab tab, AutoComplete completion)
		{
			Tab = tab;
			LanguageCompletion = completion;

			// arrange
			GlobalIdentifiers = new List<IdentifierMatch>();
			AvailableOptions = new List<AutoCompleteItem>();
			IgnoredScopes = new List<ScopeRange>();
			TokensBeforeCaret = new List<string>();

			// initialize completion worker
			completionWorker = new BackgroundWorker();
			completionWorker.DoWork += (s, e) => PopulateAutoComplete(e);
		}

		private void PopulateTokensBeforeCaret(string code, int caretOffset)
		{
			var insideParenthesis = 0;
			var index = caretOffset - 1;
			var delimiter = string.Empty;
			var sequence = new StringBuilder();
			var result = new List<string>();

			while (index >= 0)
			{
				var _char = code[index];

				if (insideParenthesis == 0)
				{
					if (_char == ')')
					{
						insideParenthesis++;
					}
					else if (char.IsLetterOrDigit(_char))
					{
						sequence.Insert(0, _char);
					}
					else if (LanguageCompletion.ObjectAttributeDelimiters.Any(d => d.Contains(_char)))
					{
						delimiter = _char + delimiter;

						if (LanguageCompletion.ObjectAttributeDelimiters.Any(d => d == delimiter))
						{
							delimiter = string.Empty;
							result.Add(sequence.ToString());
							sequence = new StringBuilder();
						}
					}
					else
					{
						result.Add(sequence.ToString());
						break;
					}
				}
				else
				{
					if (_char == '(') insideParenthesis--;
					else if (_char == ')') insideParenthesis++;
				}

				index--;
			}

			var countOfEmpty = result.Count(t => t == string.Empty);

			if (countOfEmpty >= 2 || countOfEmpty == result.Count)
				TokensBeforeCaret = new List<string>();
			else
				TokensBeforeCaret = result;
		}

		public void PopulateGlobalIdentifiers(string code, int caretOffset)
		{
			var sbCode = new StringBuilder(code);
			var scopes = new List<ScopeRange>();
			
			GlobalIdentifiers.Clear();
			
			#region Ignore sections

			IgnoredScopes = new List<ScopeRange>();

			if (LanguageCompletion.IgnoreExpressions == null)
			{
				LanguageCompletion.IgnoreExpressions = new List<Regex>();

				foreach (var section in LanguageCompletion.IgnoreSections)
					LanguageCompletion.IgnoreExpressions.Add(new Regex(section, RegexOptions.Compiled | RegexOptions.CultureInvariant));
			}

			foreach (var section in LanguageCompletion.IgnoreExpressions)
			{
				var match = section.Match(sbCode.ToString());

				while (match.Success)
				{
					for (var i = 0; i < match.Length; i++)
						sbCode[match.Index + i] = ' ';

					IgnoredScopes.Add(new ScopeRange(match.Index, match.Index + match.Length));

					match = match.NextMatch();
				}
			}

			#endregion

			#region Populate scopes

			foreach (var scope in LanguageCompletion.Scopes)
			{
				var scopeStack = new Stack<int>();
				var beginMatch = scope.BeginRegex;
				var scopeMatch = scope.ScopeMatch.Match(sbCode.ToString());

				while (scopeMatch.Success)
				{
					if (beginMatch.IsMatch(scopeMatch.Value))
					{
						scopeStack.Push(scopeMatch.Index);
					}
					else
					{
						if (scopeStack.Count > 0)
						{
							scopes.Add(new ScopeRange(scopeStack.Pop(), scopeMatch.Index));
						}
					}

					scopeMatch = scopeMatch.NextMatch();
				}
			}

			scopes.Add(new ScopeRange(0, sbCode.Length));

			#endregion

			#region Populate matches

			foreach (var identifier in LanguageCompletion.Identifiers)
			{
				var match = identifier.Regex.Match(sbCode.ToString());

				while (match.Success)
				{
					var offset = match.Index;
					var scope = scopes.Where(s => s.From < offset && s.To >= offset).OrderBy(s => s.Size).First();

					if (identifier.OptionType == "Object")
					{
						var expression = code.Substring(match.Groups["expression"].Index, match.Groups["expression"].Length);
						var type = GetPossibleTypeByInference(expression);

						GlobalIdentifiers.Add(new IdentifierMatch(CompletionItemType.Object, scope, offset, match.Groups["name"].Value, expression, type));
					}
					else if (identifier.OptionType == "Function")
					{
						var name = match.Groups["name"].Value;
						var parameters = code.Substring(match.Groups["parameters"].Index, match.Groups["parameters"].Length);
						
						if (parameters.IsBlank())
							parameters = "Parameterless";
						else
							parameters = "Parameters: " + parameters;
						
						GlobalIdentifiers.Add(new IdentifierMatch(CompletionItemType.Function, scope, offset, name, parameters, LanguageCompletion.GetFunctionType()));
					}

					// remove the match, so its not processed again
					for (var i = 0; i < match.Length; i++)
						sbCode[match.Index + i] = ' ';

					// allow for next match
					match = match.NextMatch();
				}
			}

			#endregion

			AvailableOptions = GlobalIdentifiers
				.Where(i => i.Scope.From < caretOffset && i.Scope.To >= caretOffset)
				.Distinct()
				.Select(m => new AutoCompleteItem(m.Type, m.Name, m.Expression))
				.ToList();

			#region Get options of global type

			var global = LanguageCompletion.GetGlobalType();

			if (global != null)
			{
				foreach (var option in global.Options)
				{
					AvailableOptions.Add(new AutoCompleteItem(option.GetCompletionType(), option.Name, option.Description));
				}
			}

			#endregion
		}

		public bool ShowWindow()
		{
			PopulateTokensBeforeCaret(Tab.Editor.Text, Tab.Editor.CaretOffset);

			if (IgnoredScopes.Any(s => s.IsCaretInsideScope(Tab.Editor.CaretOffset)))
			{
				return false;
			}

			if (TokensBeforeCaret.Count > 0 && AvailableOptions.Any(i => i.Text.StartsWith(TokensBeforeCaret.First())))
			{
				return true;
			}

			return false;
		}

		public void TextChanged()
		{
			if (!completionWorker.IsBusy)
			{
				completionWorker.RunWorkerAsync(new EditorState
				{
					CaretOffset = Tab.Editor.CaretOffset,
					Text = Tab.Editor.Text
				});
			}
		}

		public Type GetPossibleTypeFromParent(Type type, string token)
		{
			if (type == null)
				return null;

			var option = type.Options.FirstOrDefault(o => o.Name == token);

			if (option == null)
				return null;

			return LanguageCompletion.Types.FirstOrDefault(t => t.Name == option.ReturnType);
		}

		public Type GetPossibleTypeByInference(string expression)
		{
			foreach(var exp in LanguageCompletion.Inference)
			{
				if (exp.ExpressionRegex.Match(expression).Success)
					return exp.GetCompletionType(LanguageCompletion);
			}

			return LanguageCompletion.GetBaseType();
		}

		public void PopulateAutoComplete(DoWorkEventArgs args)
		{
			var state = args.Argument as EditorState;

			// populate identifiers
			if (TokensBeforeCaret.Count <= 1)
			{
				PopulateGlobalIdentifiers(state.Text, state.CaretOffset);
			}
			else
			{
				List<TypeOption> typeOptions = new List<TypeOption>();
				var type = LanguageCompletion.GetGlobalType();
				var baseType = LanguageCompletion.GetBaseType();
				AvailableOptions = new List<AutoCompleteItem>();
				
				for(var i = TokensBeforeCaret.Count - 1; i > 0; i--)
				{
					var token = TokensBeforeCaret[i];

					if (token.IsBlank())
					{
						break;
					}

					if ((i == TokensBeforeCaret.Count - 1) && GlobalIdentifiers.Any(g => g.Name == token))
					{
						type = GlobalIdentifiers.LastOrDefault(g => g.Name == token).InferenceType;
					}
					else
					{
						type = GetPossibleTypeFromParent(type, TokensBeforeCaret[i]);
					}
					
				}

				if (type == null)
				{
					if (baseType != null)
						typeOptions = baseType.Options;
				}
				else
				{
					if (baseType != null)
						typeOptions = type.Options.Union(LanguageCompletion.GetBaseType().Options).ToList();
				}

				foreach (var option in typeOptions)
					AvailableOptions.Add(new AutoCompleteItem(option.GetCompletionType(), option.Name, option.Description));
			}
			
			// add items
			var items = AvailableOptions.Select(i => i as CompletionWindowItem).ToList();
			Tab.Editor.Invoke(() => Tab.AddCompletionItems(AutoCompleteItem.COMPLETION_OWNER_NAME, items));
		}

		public int GetCompletionWindowOffset(string enteredText)
		{
			if (LanguageCompletion.ObjectAttributeDelimiters.Contains(enteredText))
				return Tab.Editor.CaretOffset;

			if (TokensBeforeCaret.Count > 0)
				return Tab.Editor.CaretOffset - TokensBeforeCaret.First().Length;

			return Tab.Editor.CaretOffset - enteredText.Length;
		}
	}

	internal class EditorState
	{
		public int CaretOffset { get; set; }
		public string Text { get; set; }
	}
}
