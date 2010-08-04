{
	"Name": "JavaScript",
	"Background": "#0B0A09",
	"Foreground": "#FFFFFF",
	
	"Associations": [
		"js"
	],
	
	"Spans": [
		{
			"Name": "MLComment",
			"Color": "#428BDD",
			"Start": "/\\*",
			"End": "\\*/"
		},
		{
			"Name": "SLComment",
			"Color": "#428BDD",
			"Start": "//",
			"End": ".*"
		},
		{
			"Name": "SQString",
			"Color": "#E2CE00",
			"Start": "'",
			"End": "'|$",
			"Escape": "\\"
		},
		{
			"Name": "DQString",
			"Color": "#E2CE00",
			"Start": "\"",
			"End": "\"|$",
			"Escape": "\\"
		}
	],
	
	"Rules": [
		{
			"Name": "Keyword",
			"Color": "#8AA6C1",
			"Regex": "\\b(abstract|boolean|break|byte|case|catch|char|class|const|continue|debugger|default|delete|do|double|else|enum|export|extends|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|long|native|new|package|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient|try|typeof|var|void|volatile|while|with)\\b"
		},
		{
			"Name": "Object",
			"Color": "#80D500",
			"Regex": "\\b(Array|Boolean|Date|Function|Math|Number|Object|RegExp|Global|String)\\b"
		},
		{
			"Name": "Literal",
			"Color": "#8AA6C1",
			"Regex": "\\b(true|false|null|NaN|Infinity|undefined)\\b"
		}
		
	]
}