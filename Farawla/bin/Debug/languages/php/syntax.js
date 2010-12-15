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
			Name: "comment single-line",
			Start: "\\#",
			End: ".+"
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
			Escape: "\\",
			Syntax: {
				Rules: [
					{
						Name: "object",
						Regex: "\\$([a-zA-Z_\\x7f-\\xff][a-zA-Z0-9_\\x7f-\\xff]*)"
					}
				]
			}
		}
	],
	
	Rules: [
		{
			Name: "terminal", /* they're actually premitive types, but for compatability with other langs I'll add them as terminals */
			Regex: "\\b(boolean|integer|float|string|array|object|resource|NULL|mixed|number|callback|double)\\b"
		},
		{
			Name: "object",
			Regex: "\\$([a-zA-Z_\\x7f-\\xff][a-zA-Z0-9_\\x7f-\\xff]*)"
		},
		{
			Name: "keyword",
			Regex: "\\b(abstract|and|array|as|break|case|catch|cfunction|class|clone|const|continue|declare|default|do|else|elseif|enddeclare|endfor|endforeach|endif|endswitch|endwhile|extends|final|for|foreach|function|global|goto|if|implements|interface|instanceof|namespace|new|old_function|or|private|protected|public|static|switch|throw|try|use|var|while|xor|die|echo|empty|exit|eval|include|include_once|isset|list|require|require_once|return|print|unset)\\b"
		}
	]
}