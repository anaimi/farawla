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
			Regex: "\\b(alias|begin|BEGIN|break|case|defined|do|else|elsif|end|END|ensure|for|if|in|include|loop|next|raise|redo|rescue|retry|return|super|then|undef|unless|until|when|while|yield|false|nil|self|true|__FILE__|__LINE__|and|not|or|def|class|module|catch|fail|load|throw)\\b"
		},
		{
			Name: "object",
			Regex: "\\b(Array|Boolean|Date|Function|Math|Number|Object|RegExp|Global|String)\\b"
		},
		{
			Name: "terminal",
			Regex: "\\b(true|false|null|NaN|Infinity|undefined)\\b"
		}
		
	]
}