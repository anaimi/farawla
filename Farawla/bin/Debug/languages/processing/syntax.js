/*
	Language: JavaScript
*/
{
	Spans: [
		{
			Name: "comment multi-line",
			Start: "/\\*",
			End: "\\*/"
		},
		{
			Name: "comment single-line",
			Start: "//",
			End: ".*"
		},
		{
			Name: "string single-quote",
			Start: "'",
			End: "'|$",
			Escape: "\\"
		},
		{
			Name: "string double-quote",
			Start: "\"",
			End: "\"|$",
			Escape: "\\"
		}
	],
	
	Rules: [
		{
			Name: "keyword",
			Regex: "\\b(boolean|byte|char|color|double|float|int|long|Array|ArraList|HashMap|Object|String|XMLElement|new|if|for|while|switch|class|super|break|case|continue|default|else|public|private|return|static|this|void|import|extends|final|implements)\\b"
		},
		{
			Name: "object",
			Regex: "\\b(Array|Boolean|Date|Function|Math|Number|Object|RegExp|Global|String)\\b"
		},
		{
			Name: "terminal",
			Regex: "\\b(true|false|null|PI|HALF_PI|QUARTER_PI|TWO_PI)\\b"
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