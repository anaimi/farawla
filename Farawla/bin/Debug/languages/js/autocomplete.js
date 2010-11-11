{
	Identifiers: [
		{
			OptionType: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(,)"
		},
		{
			OptionType: "Object",
			Match: "(?<name>\\w+)(\\s?)=(\\s?)(?<expression>.+)(;)"
		},
		{
			OptionType: "Function",
			Match: "function(\\s)(?<name>\\w+)(\\s?)\\((?<parameters>.*)\\)(\\s?){"
		}
	],
		
	Scopes: [
		{
			Begin: "\\{",
			End: "\\}"
		},
		{
			Begin: "\\(",
			End: "\\)"
		}
	],
	
	Types: [
		{
			Name: "_Farawla_Global",
			Options: [
				{ Name: "Infinity", Description: "A numeric value that represents positive/negative infinity", OptionType: "Object", ReturnType: "Number" },
				{ Name: "NaN", Description: "Not-a-Number value", OptionType: "Function", ReturnType: "Object" },
				{ Name: "undefined", Description: "Indicates that a variable has not been assigned a value", OptionType: "Object", ReturnType: "Number" },
				{ Name: "Math", Description: "A collection of Math functions", OptionType: "Object", ReturnType: "Math" },
				{ Name: "document", Description: "Current document object", OptionType: "Object", ReturnType: "Document" },
				{ Name: "window", Description: "Current window object", OptionType: "Object", ReturnType: "Window" },
				
				{ Name: "decodeURI()", Description: "Decodes a URI", OptionType: "Function", ReturnType: "String" },
				{ Name: "decodeURIComponent()", Description: "Decodes a URI component", OptionType: "Function", ReturnType: "String" },
				{ Name: "encodeURI()", Description: "Encodes a URI", OptionType: "Function", ReturnType: "String" },
				{ Name: "encodeURIComponent()", Description: "Encodes a URI component", OptionType: "Function", ReturnType: "String" },
				{ Name: "escape()", Description: "Encodes a string", OptionType: "Function", ReturnType: "String" },
				{ Name: "eval()", Description: "Evaluates a string and executes it as if it was script code", OptionType: "Function" },
				{ Name: "isFinite()", Description: "Determines whether a value is a finite, legal number", OptionType: "Function", ReturnType: "Boolean" },
				{ Name: "isNaN()", Description: "Determines whether a value is an illegal number", OptionType: "Function", ReturnType: "Boolean" },
				{ Name: "Number()", Description: "Converts an object's value to a number", OptionType: "Function", ReturnType: "Number" },
				{ Name: "parseFloat()", Description: "Parses a string and returns a floating point number", OptionType: "Function", ReturnType: "Number" },
				{ Name: "parseInt()", Description: "Parses a string and returns an integer", OptionType: "Function", ReturnType: "Number" },
				{ Name: "String()", Description: "Converts an object's value to a string", OptionType: "Function", ReturnType: "String" },
				{ Name: "unescape()", Description: "Decodes an encoded string", OptionType: "Function", ReturnType: "String" }
			]
		},
		{
			Name: "base",
			Options: [
				{ Name: "toString()", Description: "Returns a string representation of the current object", OptionType: "Function", ReturnType: "String" }
			]
		},
		{
			Name: "Window",
			Options: [
				{ Name: "closed", Description: "Returns a Boolean value indicating whether a window has been closed or not", OptionType: "Object", ReturnType: "Boolean" },
				{ Name: "defaultStatus", Description: "Sets or returns the default text in the statusbar of a window", OptionType: "Object", ReturnType: "String" },
				{ Name: "document", Description: "Returns the Document object for the window (See Document object)", OptionType: "Object", ReturnType: "Document" },
				{ Name: "frames", Description: "Returns an array of all the frames (including iframes) in the current window", OptionType: "Object" },
				{ Name: "history", Description: "Returns the History object for the window (See History object)", OptionType: "Object", ReturnType: "History" },
				{ Name: "innerHeight", Description: "Sets or returns the the inner height of a window's content area", OptionType: "Object", ReturnType: "Number" },
				{ Name: "innerWidth", Description: "Sets or returns the the inner width of a window's content area", OptionType: "Object", ReturnType: "Number" },
				{ Name: "length", Description: "Returns the number of frames (including iframes) in a window", OptionType: "Object", ReturnType: "Number" },
				{ Name: "location", Description: "Returns the Location object for the window (See Location object)", OptionType: "Object", ReturnType: "String" },
				{ Name: "name", Description: "Sets or returns the name of a window", OptionType: "Object", ReturnType: "String" },
				{ Name: "navigator", Description: "Returns the Navigator object for the window (See Navigator object)", OptionType: "Object", ReturnType: "Navigator" },
				{ Name: "opener", Description: "Returns a reference to the window that created the window", OptionType: "Object", ReturnType: "Window" },
				{ Name: "outerHeight", Description: "Sets or returns the outer height of a window, including toolbars/scrollbars", OptionType: "Object", ReturnType: "Number" },
				{ Name: "outerWidth", Description: "Sets or returns the outer width of a window, including toolbars/scrollbars", OptionType: "Object", ReturnType: "Number" },
				{ Name: "pageXOffset", Description: "Returns the pixels the current document has been scrolled (horizontally) from the upper left corner of the window", OptionType: "Object", ReturnType: "Number" },
				{ Name: "pageYOffset", Description: "Returns the pixels the current document has been scrolled (vertically) from the upper left corner of the window", OptionType: "Object", ReturnType: "Number" },
				{ Name: "parent", Description: "Returns the parent window of the current window", OptionType: "Object", ReturnType: "Window" },
				{ Name: "screen", Description: "Returns the Screen object for the window (See Screen object)", OptionType: "Object", ReturnType: "Screen" },
				{ Name: "screenLeft", Description: "Returns the x coordinate of the window relative to the screen", OptionType: "Object", ReturnType: "Number" },
				{ Name: "screenTop", Description: "Returns the y coordinate of the window relative to the screen", OptionType: "Object", ReturnType: "Number" },
				{ Name: "screenX", Description: "Returns the x coordinate of the window relative to the screen", OptionType: "Object", ReturnType: "Number" },
				{ Name: "screenY", Description: "Returns the y coordinate of the window relative to the screen", OptionType: "Object", ReturnType: "Number" },
				{ Name: "self", Description: "Returns the current window", OptionType: "Object", ReturnType: "Window" },
				{ Name: "status", Description: "Sets the text in the statusbar of a window", OptionType: "Object", ReturnType: "String" },
				{ Name: "top", Description: "Returns the topmost browser window", OptionType: "Object", ReturnType: "Window" },
				
				{ Name: "alert()", Description: "Displays an alert box with a message and an OK button", OptionType: "Function" },
				{ Name: "blur()", Description: "Removes focus from the current window", OptionType: "Function" },
				{ Name: "clearInterval()", Description: "Clears a timer set with setInterval()", OptionType: "Function" },
				{ Name: "clearTimeout()", Description: "Clears a timer set with setTimeout()", OptionType: "Function" },
				{ Name: "close()", Description: "Closes the current window", OptionType: "Function" },
				{ Name: "confirm()", Description: "Displays a dialog box with a message and an OK and a Cancel button", OptionType: "Function", ReturnType: "Boolean" },
				{ Name: "createPopup()", Description: "Creates a pop-up window", OptionType: "Function" },
				{ Name: "focus()", Description: "Sets focus to the current window", OptionType: "Function" },
				{ Name: "moveBy()", Description: "Moves a window relative to its current position", OptionType: "Function" },
				{ Name: "moveTo()", Description: "Moves a window to the specified position", OptionType: "Function" },
				{ Name: "open()", Description: "Opens a new browser window", OptionType: "Function" },
				{ Name: "print()", Description: "Prints the content of the current window", OptionType: "Function" },
				{ Name: "prompt()", Description: "Displays a dialog box that prompts the visitor for input", OptionType: "Function", ReturnType: "String" },
				{ Name: "resizeBy()", Description: "Resizes the window by the specified pixels", OptionType: "Function" },
				{ Name: "resizeTo()", Description: "Resizes the window to the specified width and height", OptionType: "Function" },
				{ Name: "scrollBy()", Description: "Scrolls the content by the specified number of pixels", OptionType: "Function" },
				{ Name: "scrollTo()", Description: "Scrolls the content to the specified coordinates", OptionType: "Function" },
				{ Name: "setInterval()", Description: "Calls a function or evaluates an expression at specified intervals (in milliseconds)", OptionType: "Function" },
				{ Name: "setTimeout()", Description: "Calls a function or evaluates an expression after a specified number of milliseconds", OptionType: "Function" }
			]
		},
		{
			Name: "Document",
			Options: [
				{ Name: "cookie", Description: "Returns all name/value pairs of cookies in the document", OptionType: "Object", ReturnType: "Array" },
				{ Name: "domain", Description: "Returns the domain name of the server that loaded the document", OptionType: "Object", ReturnType: "String" },
				{ Name: "referrer", Description: "Returns the URL of the document that loaded the current document", OptionType: "Object", ReturnType: "String" },
				{ Name: "title", Description: "Sets or returns the title of the document", OptionType: "Object", ReturnType: "String" },
				{ Name: "URL", Description: "Returns the full URL of the document", OptionType: "Object", ReturnType: "String" },
				{ Name: "URL", Description: "Returns the full URL of the document", OptionType: "Object", ReturnType: "String" },
				
				{ Name: "close", Description: "Closes the output stream previously opened with document.open()", OptionType: "Function" },
				{ Name: "getElementById", Description: "Accesses the first element with the specified id", OptionType: "Function" },
				{ Name: "getElementsByName", Description: "Accesses all elements with a specified name", OptionType: "Function" },
				{ Name: "getElementsByTagName", Description: "Accesses all elements with a specified tagname", OptionType: "Function" },
				{ Name: "open", Description: "Opens an output stream to collect the output from document.write() or document.writeln()", OptionType: "Function" },
				{ Name: "write", Description: "Writes HTML expressions or JavaScript code to a document", OptionType: "Function" },
				{ Name: "writeln", Description: "Same as write(), but adds a newline character after each statement", OptionType: "Function" }
			]
		},
		{
			Name: "String",
			Options: [
				{ Name: "length", Description: "Returns the number of characters in the string", OptionType: "Object", ReturnType: "Number" },
				
				{ Name: "charAt", Description: "Returns the character at the specified index", OptionType: "Function", ReturnType: "String" },
				{ Name: "charCodeAt", Description: "Returns the Unicode of the character at the specified index", OptionType: "Function", ReturnType: "Number" },
				{ Name: "concat", Description: "Joins two or more strings, and returns a copy of the joined strings", OptionType: "Function", ReturnType: "String" },
				{ Name: "fromCharCode", Description: "Converts Unicode values to characters", OptionType: "Function", ReturnType: "String" },
				{ Name: "indexOf", Description: "Returns the position of the first found occurrence of a specified value in a string", OptionType: "Function", ReturnType: "Number" },
				{ Name: "lastIndexOf", Description: "Returns the position of the last found occurrence of a specified value in a string", OptionType: "Function", ReturnType: "Number" },
				{ Name: "match", Description: "Searches for a match between a regular expression and a string, and returns the matches", OptionType: "Function" },
				{ Name: "replace", Description: "Searches for a match between a substring (or regular expression) and a string, and replaces the matched substring with a new substring", OptionType: "Function", ReturnType: "String" },
				{ Name: "search", Description: "Searches for a match between a regular expression and a string, and returns the position of the match", OptionType: "Function", ReturnType: "Number" },
				{ Name: "slice", Description: "Extracts a part of a string and returns a new string", OptionType: "Function", ReturnType: "String" },
				{ Name: "split", Description: "Splits a string into an array of substrings", OptionType: "Function", ReturnType: "Array" },
				{ Name: "substr", Description: "Extracts the characters from a string, beginning at a specified start position, and through the specified number of character", OptionType: "Function", ReturnType: "String" },
				{ Name: "substring", Description: "Extracts the characters from a string, between two specified indices", OptionType: "Function", ReturnType: "String" },
				{ Name: "toLowerCase", Description: "Converts a string to lowercase letters", OptionType: "Function", ReturnType: "String" },
				{ Name: "toUpperCase", Description: "Converts a string to uppercase letters", OptionType: "Function", ReturnType: "String" },
				{ Name: "valueOf", Description: "Returns the primitive value of a String object", OptionType: "Function", ReturnType: "String" }
				
			]
		},
		{
			Name: "Number",
			Options: [
				{ Name: "MAX_VALUE", Description: "Returns the largest number possible in JavaScript", OptionType: "Object", ReturnType: "Number" },
				{ Name: "MIN_VALUE", Description: "Returns the smallest number possible in JavaScript", OptionType: "Object", ReturnType: "Number" },
				{ Name: "NEGATIVE_INFINITY", Description: "Represents negative infinity (returned on overflow)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "POSITIVE_INFINITY", Description: "Represents infinity (returned on overflow)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "prototype", Description: "Allows you to add properties and methods to an object", OptionType: "Object" },
				
				{ Name: "toExponential(x)", Description: "Converts a number into an exponential notation", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toFixed(x)", Description: "Formats a number with x numbers of digits after the decimal point", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toPrecision(x)", Description: "Formats a number to x length", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toString()", Description: "Converts a Number object to a string", OptionType: "Function", ReturnType: "String" },
				{ Name: "valueOf()", Description: "Returns the primitive value of a Number object", OptionType: "Function" }
			]
		},
		{
			Name: "Math",
			Options: [
				{ Name: "E", Description: "Returns Euler's number (approx. 2.718)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "LN2", Description: "Returns the natural logarithm of 2 (approx. 0.693)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "LN10", Description: "Returns the natural logarithm of 10 (approx. 2.302)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "LOG2E", Description: "Returns the base-2 logarithm of E (approx. 1.442)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "LOG10E", Description: "Returns the base-10 logarithm of E (approx. 0.434)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "PI", Description: "Returns PI (approx. 3.14159)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "SQRT1_2", Description: "Returns the square root of 1/2 (approx. 0.707)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "SQRT2", Description: "Returns the square root of 2 (approx. 1.414)", OptionType: "Object", ReturnType: "Number" },

				{ Name: "abs(x)", Description: "Returns the absolute value of x", OptionType: "Function", ReturnType: "Number" },
				{ Name: "acos(x)", Description: "Returns the arccosine of x, in radians", OptionType: "Function", ReturnType: "Number" },
				{ Name: "asin(x)", Description: "Returns the arcsine of x, in radians", OptionType: "Function", ReturnType: "Number" },
				{ Name: "atan(x)", Description: "Returns the arctangent of x as a numeric value between -PI/2 and PI/2 radians", OptionType: "Function", ReturnType: "Number" },
				{ Name: "atan2(y,x)", Description: "Returns the arctangent of the quotient of its arguments", OptionType: "Function", ReturnType: "Number" },
				{ Name: "ceil(x)", Description: "Returns x, rounded upwards to the nearest integer", OptionType: "Function", ReturnType: "Number" },
				{ Name: "cos(x)", Description: "Returns the cosine of x (x is in radians)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "exp(x)", Description: "Returns the value of Ex", OptionType: "Function", ReturnType: "Number" },
				{ Name: "floor(x)", Description: "Returns x, rounded downwards to the nearest integer", OptionType: "Function", ReturnType: "Number" },
				{ Name: "log(x)", Description: "Returns the natural logarithm (base E) of x", OptionType: "Function", ReturnType: "Number" },
				{ Name: "max(x,y,z,...,n)", Description: "Returns the number with the highest value", OptionType: "Function", ReturnType: "Number" },
				{ Name: "min(x,y,z,...,n)", Description: "Returns the number with the lowest value", OptionType: "Function", ReturnType: "Number" },
				{ Name: "pow(x,y)", Description: "Returns the value of x to the power of y", OptionType: "Function", ReturnType: "Number" },
				{ Name: "random()", Description: "Returns a random number between 0 and 1", OptionType: "Function", ReturnType: "Number" },
				{ Name: "round(x)", Description: "Rounds x to the nearest integer", OptionType: "Function", ReturnType: "Number" },
				{ Name: "sin(x)", Description: "Returns the sine of x (x is in radians)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "sqrt(x)", Description: "Returns the square root of x", OptionType: "Function", ReturnType: "Number" },
				{ Name: "tan(x)", Description: "Returns the tangent of an angle", OptionType: "Function", ReturnType: "Number" }
			]
		},
		{
			Name: "Array",
			Options: [
				{ Name: "length", Description: "Sets or returns the number of elements in an array", OptionType: "Object", ReturnType: "Number" },
				{ Name: "prototype", Description: "Allows you to add properties and methods to an object", OptionType: "Object" },
				
				{ Name: "concat()", Description: "Joins two or more arrays, and returns a copy of the joined arrays", OptionType: "Function", ReturnType: "Array" },
				{ Name: "join()", Description: "Joins all elements of an array into a string", OptionType: "Function", ReturnType: "String" },
				{ Name: "pop()", Description: "Removes the last element of an array, and returns that element", OptionType: "Function" },
				{ Name: "push()", Description: "Adds new elements to the end of an array, and returns the new length", OptionType: "Function", ReturnType: "Number" },
				{ Name: "reverse()", Description: "Reverses the order of the elements in an array", OptionType: "Function" },
				{ Name: "shift()", Description: "Removes the first element of an array, and returns that element", OptionType: "Function" },
				{ Name: "slice()", Description: "Selects a part of an array, and returns the new array", OptionType: "Function", ReturnType: "Array" },
				{ Name: "sort()", Description: "Sorts the elements of an array", OptionType: "Function" },
				{ Name: "splice()", Description: "Adds/Removes elements from an array", OptionType: "Function" },
				{ Name: "toString()", Description: "Converts an array to a string, and returns the result", OptionType: "Function", ReturnType: "String" },
				{ Name: "unshift()", Description: "Adds new elements to the beginning of an array, and returns the new length", OptionType: "Function", ReturnType: "Number" }
			]
		},
		{
			Name: "Boolean",
			Options: [
				{ Name: "prototype", Description: "Allows you to add properties and methods to an object", OptionType: "Object", ReturnType: "Number" },
				{ Name: "toString()", Description: "Converts a Boolean value to a string, and returns the result", OptionType: "Function", ReturnType: "String" },
				{ Name: "valueOf()", Description: "Returns the primitive value of a Boolean objec", OptionType: "Function", ReturnType: "Number" }
			]
		},
		{
			Name: "Date",
			Options: [
				{ Name: "prototype", Description: "Allows you to add properties and methods to an object", OptionType: "Object", ReturnType: "Number" },
				{ Name: "getDate()", Description: "Returns the day of the month (from 1-31)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getDay()", Description: "Returns the day of the week (from 0-6)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getFullYear()", Description: "Returns the year (four digits)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getHours()", Description: "Returns the hour (from 0-23)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getMilliseconds()", Description: "Returns the milliseconds (from 0-999)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getMinutes()", Description: "Returns the minutes (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getMonth()", Description: "Returns the month (from 0-11)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getSeconds()", Description: "Returns the seconds (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getTime()", Description: "Returns the number of milliseconds since midnight Jan 1, 1970", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getTimezoneOffset()", Description: "Returns the time difference between GMT and local time, in minutes", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCDate()", Description: "Returns the day of the month, according to universal time (from 1-31)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCDay()", Description: "Returns the day of the week, according to universal time (from 0-6)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCFullYear()", Description: "Returns the year, according to universal time (four digits)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCHours()", Description: "Returns the hour, according to universal time (from 0-23)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCMilliseconds()", Description: "Returns the milliseconds, according to universal time (from 0-999)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCMinutes()", Description: "Returns the minutes, according to universal time (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCMonth()", Description: "Returns the month, according to universal time (from 0-11)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "getUTCSeconds()", Description: "Returns the seconds, according to universal time (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "parse()", Description: "Parses a date string and returns the number of milliseconds since midnight of January 1, 1970", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setDate()", Description: "Sets the day of the month (from 1-31)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setFullYear()", Description: "Sets the year (four digits)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setHours()", Description: "Sets the hour (from 0-23)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setMilliseconds()", Description: "Sets the milliseconds (from 0-999)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setMinutes()", Description: "Set the minutes (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setMonth()", Description: "Sets the month (from 0-11)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setSeconds()", Description: "Sets the seconds (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setTime()", Description: "Sets a date and time by adding or subtracting a specified number of milliseconds to/from midnight January 1, 1970", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCDate()", Description: "Sets the day of the month, according to universal time (from 1-31)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCFullYear()", Description: "Sets the year, according to universal time (four digits)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCHours()", Description: "Sets the hour, according to universal time (from 0-23)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCMilliseconds()", Description: "Sets the milliseconds, according to universal time (from 0-999)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCMinutes()", Description: "Set the minutes, according to universal time (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCMonth()", Description: "Sets the month, according to universal time (from 0-11)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "setUTCSeconds()", Description: "Set the seconds, according to universal time (from 0-59)", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toDateString()", Description: "Converts the date portion of a Date object into a readable string", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toLocaleDateString()", Description: "Returns the date portion of a Date object as a string, using locale conventions", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toLocaleTimeString()", Description: "Returns the time portion of a Date object as a string, using locale conventions", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toLocaleString()", Description: "Converts a Date object to a string, using locale conventions", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toString()", Description: "Converts a Date object to a string", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toTimeString()", Description: "Converts the time portion of a Date object to a string", OptionType: "Function", ReturnType: "Number" },
				{ Name: "toUTCString()", Description: "Converts a Date object to a string, according to universal time", OptionType: "Function", ReturnType: "Number" },
				{ Name: "UTC()", Description: "Returns the number of milliseconds in a date string since midnight of January 1, 1970, according to universal time", OptionType: "Function", ReturnType: "Number" },
				{ Name: "valueOf()", Description: "Returns the primitive value of a Date object", OptionType: "Function", ReturnType: "Number" }
			]
		}
	],
	
	IgnoreSections: [
		"\\\".*?\\\"", /* double quoted strings */
		"'.*?'", /* single quoted strings */
		"//.+", /* line comments */
		"/\\*(.|\\n|\\r)+\\*/" /* block comments */
	],
	
	GlobalTypeName: "_Farawla_Global",
	BaseTypeName: "base",
	ObjectAttributeDelimiters: ["."]
}