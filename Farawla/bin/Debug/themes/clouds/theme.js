/*
	Clouds
	By Ahmad Al-Naimi
	Heavily inspired from http://fredhq.com/projects/clouds
*/

{
	/* Background colors for document tab headers */
	WindowTabSelectedColor: "#FFFFFFFF,#FFF4F8FE",
	WindowTabHoverColor: "#FFB0B0B0,#FFA0A0A0",
	WindowTabInactiveColor: "#FFA0A0A0,#FF909090",
	
	/* Text colors for document tab headers */
	WindowTabSelectedCaptionColor: "#FF000000",
	WindowTabInactiveCaptionColor: "#FF000000",
	
	/* Color for the main tab, the thin border between the tab header and the editor */
	WindowTabToolbarColor: "#FFF4F8FE",
	
	/* Background of characters matching the current highlighted alpha-num characters */
	MatchingTokensBackground: "#FFFFFF00",
	
	/* Background of bracket pairs when caret is next to a bracket */
	MatchingBracketsBackground: "#FFE2F7F9",
	
	/* If Show Tabs is enabled (from Settings) a line will appear over every tab */
	ShowTabColor: "#FF86B626",
	
	/* If Show Spaces is enabled (from Settings) a dot will appear over every space character */
	ShowSpaceColor: "#FFFF0000", /* if Show Spaces in enabled, dots of this color will be drawn */
	
	/* Background of the line hosting the caret */
	HighlightLineOfCaret: true,
	LineOfCaretColor: "#22FFFF00",
	
	/*
		The widgets (in the sidebar) are colored using two colors
		It's better to keep these semi-transparent as there's a lot of layering
		Usually 'Primary' would be used for the background and 'Secondary' for the borders - this rule is not always followed
	*/
	PrimaryWidgetColor: "#883769A0",
	SecondaryWidgetColor: "#11000000",
	
	/* Color Text in the widgets */
	TextWidgetColor: "#FFFFFFFF",
	
	/* Completion window background and foreground (text) */
	CompletionWindowBackground: "#553769A0,#883769A0",
	CompletionWindowForeground: "#FFFFFFFF",
	
	/* Editor background and foreground (text) */
	Background: "#FFFFFFFF",
	Foreground: "#FF000000",
	
	/*
		Text highlighting:
		Each pair represent the name of the syntax and the color of the text recognized as part of that syntax.
		So if 'comment' is given the color #00FF00, then all comments (in all languages) will be colored in Red.
		
		Note about syntax names:
			If a name contains a space, such as 'comment block', then the space will be interpreted as 'more specifically'.
			Meaning, the engine will attempt to find the color 'comment block', but if it doesn't find it, it
			will remove the last token and just search for 'comment'...
			So as a theme designer, it's better to be as general as possible.
	*/
	SyntaxColors: {
		"comment": "#BCC8BA",
		"string": "#5D90CD",
		"keyword": "#AF956F",
		"object": "#39946A",
		"terminal": "#39946A",
		"symbol": "#39946A",
		"number": "#46A609",
		"operator": "#CCCCCC",
		"regex": "#C2515A",
		"xml-tag": "#AF956F",
		"xml-attribute": "#39946A",
		"css-block": "#39946A",
		"css-property": "#AF956F",
		"css-value": "#5D90CD"
	}
}