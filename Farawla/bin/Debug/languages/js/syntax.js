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
			End: "$"
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
			Regex: "\\b(abstract|boolean|break|byte|case|catch|char|class|const|continue|debugger|default|delete|do|double|else|enum|export|extends|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|long|native|new|package|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient|try|typeof|var|void|volatile|while|with)\\b"
		},
		{
			Name: "object",
			Regex: "\\b(Array|Boolean|Date|Function|Math|Number|Object|RegExp|Global|String)\\b"
		},
		{
			Name: "terminal",
			Regex: "\\b(true|false|null|NaN|Infinity|undefined)\\b"
		},
		{
			Name: "number",
			Regex: "((0x)?(\\d+)(\\.\\d+)?(f)?)"
		},
		{
			Name: "regex",
			Regex: "\\(/.+/[gim]*\\)"
		}
		
	]
}