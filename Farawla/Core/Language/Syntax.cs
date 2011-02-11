using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Farawla.Core.Language
{
	public class Syntax
	{
		public List<string> IncludeLanguageSyntax { get; set; }
		public List<Rule> Rules { get; set; }
		public List<Span> Spans { get; set; }

		private HighlightingDefinition highlighter;

		public Syntax()
		{
			IncludeLanguageSyntax = new List<string>();
			Rules = new List<Rule>();
			Spans = new List<Span>();
		}

		public void Initialize(string name)
		{
			highlighter = new HighlightingDefinition(name);
			
			// merge included languages
			foreach(var langName in IncludeLanguageSyntax)
			{
				var lang = Controller.Current.Languages.GetLanguageByName(langName);
				
				if (lang.IsDefault)
					continue;

				Merge(lang.Syntax);
			}
			
			// add self rule set
			PopulateRuleSet(highlighter.MainRuleSet);
		}

		private void Merge(Syntax syntax)
		{
			foreach(var rule in syntax.Rules)
				Rules.Add(rule);
			
			foreach(var span in syntax.Spans)
				Spans.Add(span);
		}

		public void PopulateRuleSet(HighlightingRuleSet ruleSet)
		{
			// rules
			if (Rules != null)
			{
				foreach (var rule in Rules)
					ruleSet.Rules.Add(rule.GetRule());
			}

			// spans
			if (Spans != null)
			{
				foreach (var span in Spans)
					ruleSet.Spans.Add(span.GetSpan());
			}
		}

		public HighlightingDefinition GetHighlighter()
		{
			return highlighter;
		}
	}
	
	public class Rule : GenericRule
	{
		public string Regex { get; set; }

		public HighlightingRule GetRule()
		{
			var rule = new HighlightingRule();

			rule.Color = GetColor();
			rule.Regex = GetRegexFromString(Regex);

			return rule;
		}
	}

	public class Span : GenericRule
	{
		public string Start { get; set; }
		public string End { get; set; }
		public string Escape { get; set; }
		public string Reference { get; set; }

		private HighlightingRuleSet ruleSet;
		public Syntax Syntax { get; set; }

		public HighlightingSpan GetSpan()
		{
			var span = new HighlightingSpan();

			span.SpanColor = GetColor();
			span.StartExpression = GetRegexFromString(Start);
			span.EndExpression = GetRegexFromString(End);
			span.RuleSet = GetRuleSetFromSyntax();
			
			if (Reference.IsBlank())
			{
				span.StartColor = span.EndColor = span.SpanColor;
			}
			
			if (!Escape.IsBlank())
			{
				if (Escape == "\\")
					Escape = "\\\\";
				
				if (span.RuleSet == null)
					span.RuleSet = new HighlightingRuleSet();
				
				span.RuleSet.Spans.Add(new HighlightingSpan {
					StartExpression = GetRegexFromString(Escape),
					EndExpression = GetRegexFromString(".")
				});
			}
			//TODO: use escape character

			return span;
		}
		
		private HighlightingRuleSet GetRuleSetFromSyntax()
		{
			if (Syntax == null && Reference.IsBlank())
				return null;

			if (ruleSet == null)
			{
				if (Syntax != null)
				{
					ruleSet = new HighlightingRuleSet();
					Syntax.PopulateRuleSet(ruleSet);
				}
				else
				{
					var lang = Controller.Current.Languages.GetLanguageByName(Reference);
					
					if (!lang.HasSyntax)
					{
						Reference = null;
						return null;
					}
					
					if (!lang.HaveInitializedChildren)
					{
						lang.InitializeChildren();
					}

					ruleSet = lang.Syntax.GetHighlighter().MainRuleSet;
				}
			}

			return ruleSet;

		}
	}

	public class GenericRule
	{
		public string Name { get; set; }

		protected FontWeight GetFontWeight()
		{
			return FontWeights.Normal;
		}

		protected FontStyle GetFontStyle()
		{
			return FontStyles.Normal;
		}

		protected HighlightingCustomeBrush GetForeground()
		{
			return new HighlightingCustomeBrush(new SolidColorBrush(Theme.Instance.GetColor(Name)));
		}

		protected HighlightingColor GetColor()
		{
			var color = new HighlightingColor();

			color.Foreground = GetForeground();
			color.FontWeight = GetFontWeight();
			color.FontStyle = GetFontStyle();
			
			return color;
		}
		
		protected Regex GetRegexFromString(string pattern)
		{
			return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
		}

		public override string ToString()
		{
			return Name ?? "";
		}
	}

	public class HighlightingDefinition : IHighlightingDefinition
	{
		private string _name;
		private HighlightingRuleSet _rules;

		public HighlightingDefinition(string name)
		{
			_name = name;
			_rules = new HighlightingRuleSet();
		}

		public HighlightingColor GetNamedColor(string name)
		{
			return null;
		}

		public HighlightingRuleSet GetNamedRuleSet(string name)
		{
			return null;
		}

		public HighlightingRuleSet MainRuleSet
		{
			get { return _rules; }
		}

		public IEnumerable<HighlightingColor> NamedHighlightingColors
		{
			get
			{
				foreach (var rule in _rules.Rules)
				{
					yield return rule.Color;
				}

				foreach (var span in _rules.Spans)
				{
					yield return span.SpanColor;
				}
			}
		}

		public string Name
		{
			get { return _name; }
		}
	}

	public class HighlightingCustomeBrush : HighlightingBrush
	{
		private Brush _brush;

		public HighlightingCustomeBrush(Brush brush)
		{
			_brush = brush;
		}

		public override Brush GetBrush(ICSharpCode.AvalonEdit.Rendering.ITextRunConstructionContext context)
		{
			return _brush;
		}
	}
}
