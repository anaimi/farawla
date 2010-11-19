/*
	In SyntaxColors, you specify the name of the syntax. The name here is the same in "syntax.js" file inside a language file.
	Example, if you defined a "comment" span inside "syntax.js", then all comment spans will be colored as specified by you in this file.
	
	Also note, that spaces in a color's name means 'more specifically' ... for example, let's say you have the span "comment multi-line". The editor
	will try to find the color "comment multi-line" from the SyntaxColors below. If not found, it will less restrictive by removing the last token,
	making the new name "comment" and searching for that color. This way enables theme designers to generalize and specialize colors as needed.
*/

{
	MatchingTokensBackground: "#FFFFFF00", /* when highlighting an alpha-num sequence of characters, the editor will change the background of matching text to this color -- also, used to highlight results when searching before hitting enter */
	MatchingBracketsBackground: "#FFE2F7F9", /* when caret is next to a bracket, it will highlgiht the backgrounds of the pair. A bracket is one of: (,),{,},[,] */
	
	TabColor: "#FF86B626", /* if Show Tabs is enabled, lines of this color whill be drawn */
	SpaceColor: "#FFFF0000", /* if Show Spaces in enabled, dots of this color will be drawn */
	
	PrimaryWidgetColor: "#44606060",
	SecondaryWidgetColor: "#22C0C0C0",
	TextWidgetColor: "#FFFFFFFF",
	
	CompletionWindowBackground: "#919AD429",
	CompletionWindowForeground: "#FFFF0000",
	
	Background: "#FF606060",
	Foreground: "#FFFFFFFF",
	
	SyntaxColors: {
		"comment": "#BCC8BA",
		"string": "#5D90CD",
		"keyword": "#AF956F",
		"object": "#39946A",
		"terminal": "#39946A",
		"number": "#46A609",
		"operator": "#CCCCCC",
		"symbol": "#CCCCCC"
	}
}