/*
	Language: Ruby
*/
{
	Spans: [
		{
			Name: "comment multi-line",
			Start: "=begin",
			End: "=end"
		},
		{
			Name: "comment single-line",
			Start: "\\#",
			End: "$"
		},
		{
			Name: "string single-quote",
			Start: "'",
			End: "'|$",
			Escape: "\\",
			Syntax: {
				Spans: [
					{
						Name: "object",
						Start: "\\#\\{",
						End: "\\}"
					}
				]
			}
		},
		{
			Name: "string double-quote",
			Start: "\"",
			End: "\"|$",
			Escape: "\\",
			Syntax: {
				Spans: [
					{
						Name: "object",
						Start: "\\#\\{",
						End: "\\}"
					}
				]
			}
		},
		{
			Name: "string percentage",
			Start: "%{",
			End: "}",
			Syntax: {
				Spans: [
					{
						Name: "object",
						Start: "\\#\\{",
						End: "\\}"
					}
				]
			}
		}
	],
	
	Rules: [
		{
			Name: "keyword",
			Regex: "\\b(alias|begin|BEGIN|break|case|defined|do|else|elsif|end|END|ensure|for|if|in|include|loop|next|raise|redo|rescue|retry|return|super|then|undef|unless|until|when|while|yield|false|nil|self|true|__FILE__|__LINE__|and|not|or|def|class|module|catch|fail|load|throw|require)\\b"
		},
		{
			Name: "type",
			Regex: "([A-Z])(\\w|-)+"
		},
		{
			Name: "object",
			Regex: "\\@(\\w|-)+"
		},
		{
			Name: "symbol",
			Regex: "\\:(\\w|-)+"
		},
		{
			Name: "terminal",
			Regex: "\\b(true|false|nil)\\b"
		},
		{
			Name: "number",
			Regex: "((0x)?(\\d+)(\\.\\d+)?(f)?)"
		},
		{
			Name: "regex",
			Regex: "\\/(.)+\\/"
		}
		
	]
}