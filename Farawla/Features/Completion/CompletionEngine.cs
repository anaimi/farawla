using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Farawla.Core.TabContext;
using Farawla.Core;

namespace Farawla.Features.Completion
{
	public class CompletionEngine
	{
		public EditorSegment Segment { get; set; }
		public AutoComplete LanguageCompletion { get; set; }
		
		public List<IdentifierMatch> GlobalIdentifiers { get; set; }
		public List<AutoCompleteItem> AvailableOptions { get; set; }
		public List<string> TokensBeforeCaret { get; set; }
		
		public Tab ActiveTab
		{
			get { return Controller.Current.ActiveTab; }
		}
		
		public bool IsEnabled
		{
			get
			{
				return LanguageCompletion != null && Segment != null;
			}
		}

		private BackgroundWorker completionWorker;		
		
		public CompletionEngine()
		{
			Reset(null, null);

			// initialize completion worker
			completionWorker = new BackgroundWorker();
			completionWorker.DoWork += (s, e) => PopulateAutoComplete(e);
		}
		
		public void Reset(EditorSegment segment, AutoComplete completion)
		{
			Segment = segment;
			LanguageCompletion = completion;			
			
			GlobalIdentifiers = new List<IdentifierMatch>();
			AvailableOptions = new List<AutoCompleteItem>();
			TokensBeforeCaret = new List<string>();
		}
		
		public void TextChanged()
		{
			if (!completionWorker.IsBusy)
			{
				completionWorker.RunWorkerAsync(new EditorState
				{
					CaretOffset = ActiveTab.Editor.CaretOffset - Segment.Offset,
					Text = Segment.GetText()
				});
			}
		}
		
		//
		
		public void PopulateTokensBeforeCaret()
		{
			var code = Segment.GetText();
			var caretOffset = ActiveTab.Editor.CaretOffset - Segment.Offset;
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
					else if (IsIdentifierCharacter(_char))
					{
						sequence.Insert(0, _char);
					}
					else if (LanguageCompletion.ObjectAttributeDelimiters.Any(d => d.Contains(_char)))
					{
						var newDelimiter = _char + delimiter;

						if (LanguageCompletion.ObjectAttributeDelimiters.Any(d => d == newDelimiter))
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
				var typeOptions = new List<TypeOption>();
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
			ActiveTab.Editor.Invoke(() => ActiveTab.AddCompletionItems(AutoCompleteItem.COMPLETION_OWNER_NAME, items));
		}
		
		//
		
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
			foreach (var exp in LanguageCompletion.Inference)
			{
				if (exp.ExpressionRegex.Match(expression).Success)
					return exp.GetCompletionType(LanguageCompletion);
			}

			return LanguageCompletion.GetBaseType();
		}
		
		//
		
		public bool CanShowWindow()
		{
			if (IsInIgnoredSection())
			{
				return false;
			}

			PopulateTokensBeforeCaret();

			if (TokensBeforeCaret.Count > 0 && AvailableOptions.Any(i => i.Text.StartsWith(TokensBeforeCaret.First())))
			{
				return true;
			}

			return false;
		}
		
		public bool IsInIgnoredSection()
		{
			var segments = ActiveTab.GetCurrentSegments();

			foreach(var ignored in LanguageCompletion.IgnoreSections)
			{
				foreach (var segment in segments)
				{
					if (segment.Name.StartsWith(ignored))
						return true;
				}
			}

			return false;
		}
		
		public bool IsIdentifierCharacter(char c)
		{
			if (char.IsLetterOrDigit(c))
				return true;
			
			if (LanguageCompletion.AllowableIdentifierCharacters.Contains(c.ToString()))
				return true;

			return false;
		}
	}

	internal class EditorState
	{
		public int CaretOffset { get; set; }
		public string Text { get; set; }
	}
}
