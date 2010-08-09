using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.Diagnostics;
using System.Xml.XPath;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Farawla.Features.Notifier;

namespace Farawla.Core
{
	public class Languages
	{
		public List<Language> List { get; private set; }
		
		public Languages()
		{
			List = new List<Language>();
			
			if (!VerifyLanguagesFolderExist())
				return;
			
			foreach(var lang in Directory.GetDirectories("languages"))
			{
				var json = File.ReadAllText(lang + "\\language.js");
				var obj = JsonConvert.DeserializeObject<Language>(json);
				
				obj.Initialize();
				List.Add(obj);
			}
		}
		
		public Language GetLanguage(string extension)
		{
			foreach(var lang in List)
				if (lang.Associations.Any(a => a == extension))
					return lang;
			
			return new Language();
		}
		
		private bool VerifyLanguagesFolderExist()
		{
			if (!Directory.Exists("languages"))
			{
				Notifier.Instance.Show("The folder 'languages' was not found in the same directory of the executable. You should create it and load it with a folder for each language you want Farawla to support");
				return false;
			}

			if (Directory.GetDirectories("languages").Length == 0)
			{
				Notifier.Instance.Show("The folder 'languages' is empty. You should load it with folders of each language you want Farawla to support");
				return false;
			}
			
			return true;
		}
	}
	
	public class Language
	{
		public string Name { get; set; }
		public string Background { get; set; }
		public string Foreground { get; set; }
		public string FontFamily { get; set; }
		public List<string> Associations { get; set; }
		public List<Rule> Rules { get; set; }
		public List<Span> Spans { get; set; }

		private HighlightingDefinition highlighter;
		
		public Language()
		{
			Name = "Default";
			
			FontFamily = Settings.Instance.DefaultEditorFontFamily;
			Background = Settings.Instance.DefaultEditorBackground;
			Foreground = Settings.Instance.DefaultEditorForeground;
			
			Associations = new List<string>();
			Rules = new List<Rule>();
			Spans = new List<Span>();
		}
		
		public void Initialize()
		{
			highlighter = new HighlightingDefinition(Name);
			
			// rules
			if (Rules != null)
			{
				foreach (var rule in Rules)
					highlighter.MainRuleSet.Rules.Add(rule.GetRule());
			}
			
			// spans
			if (Spans != null)
			{
				foreach (var span in Spans)
					highlighter.MainRuleSet.Spans.Add(span.GetSpan());
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
			rule.Regex = new Regex(Regex);
			
			return rule;
		}
	}
	
	public class Span : GenericRule
	{
		public string Start { get; set; }
		public string End { get; set; }
		public string Escape { get; set; }

		public HighlightingSpan GetSpan()
		{
			var span = new HighlightingSpan();

			span.SpanColor = span.StartColor = span.EndColor = GetColor();
			span.StartExpression = new Regex(Start);
			span.EndExpression = new Regex(End);
			//TODO: use escape character
			
			return span;
		}
	}
	
	public class GenericRule
	{
		public string Name { get; set; }
		public string Color { get; set; }
		public string Weight { get; set; }
		public string Style { get; set; }
		
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
			return new HighlightingCustomeBrush(new SolidColorBrush(Color.ToColor()));
		}
		
		protected HighlightingColor GetColor()
		{
			var color = new HighlightingColor();
			
			color.Foreground = GetForeground();
			color.FontWeight = GetFontWeight();
			color.FontStyle = GetFontStyle();
			
			return color;
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
