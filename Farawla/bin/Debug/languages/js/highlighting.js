{
	"Name": "JavaScript",
	"Associations": [
		"js"
	],
	
	"Spans": [
		{
			"Name": "MLComment",
			"Start": "/\\*",
			"End": "\\*/"
		},
		{
			"Name": "SLComment",
			"Start": "//",
			"End": ".*"
		},
		{
			"Name": "SQString",
			"Start": "'",
			"End": "'|$",
			"Escape": "\\"
		},
		{
			"Name": "DQString",
			"Start": "\"",
			"End": "\"|$",
			"Escape": "\\"
		}
	],
	
	"Rules": [
		{
			"Name": "Keyword",
			"Regex": "\\b(abstract|boolean|break|byte|case|catch|char|class|const|continue|debugger|default|delete|do|double|else|enum|export|extends|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|long|native|new|package|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient|try|typeof|var|void|volatile|while|with)\\b"
		},
		{
			"Name": "Object",
			"Regex": "\\b(Array|Boolean|Date|Function|Math|Number|Object|RegExp|Global|String)\\b"
		},
		{
			"Name": "Terminal",
			"Regex": "\\b(true|false|null|NaN|Infinity|undefined)\\b"
		}
		
	]
}