{
	Types: [
		{
			Name: "_Farawla_Global",
			Options: [
				{ Name: "document", Description: "Current document object", OptionType: "Object", ReturnType: "Document" },
				{ Name: "window", Description: "Current window object", OptionType: "Object", ReturnType: "Window" },
				{ Name: "navigator", Description: "Current navigator object", OptionType: "Object", ReturnType: "Navigator" },
				{ Name: "history", Description: "Current history object", OptionType: "Object", ReturnType: "History" },
				{ Name: "screen", Description: "Current screen object", OptionType: "Object", ReturnType: "Screen" },
				{ Name: "location", Description: "Current location object", OptionType: "Object", ReturnType: "Location" },
			]
		},
		{
			Name: "Window",
			Options: [
				{ Name: "closed", Description: "Returns a Boolean value indicating whether a window has been closed or not", OptionType: "Object", ReturnType: "Boolean" },
				{ Name: "defaultStatus", Description: "Sets or returns the default text in the statusbar of a window", OptionType: "Object", ReturnType: "String" },
				{ Name: "document", Description: "Returns the Document object for the window", OptionType: "Object", ReturnType: "Document" },
				{ Name: "frames", Description: "Returns an array of all the frames (including iframes) in the current window", OptionType: "Object" },
				{ Name: "history", Description: "Returns the History object for the window", OptionType: "Object", ReturnType: "History" },
				{ Name: "innerHeight", Description: "Sets or returns the the inner height of a window's content area", OptionType: "Object", ReturnType: "Number" },
				{ Name: "innerWidth", Description: "Sets or returns the the inner width of a window's content area", OptionType: "Object", ReturnType: "Number" },
				{ Name: "length", Description: "Returns the number of frames (including iframes) in a window", OptionType: "Object", ReturnType: "Number" },
				{ Name: "location", Description: "Returns the Location object for the window", OptionType: "Object", ReturnType: "Location" },
				{ Name: "name", Description: "Sets or returns the name of a window", OptionType: "Object", ReturnType: "String" },
				{ Name: "navigator", Description: "Returns the Navigator object for the window", OptionType: "Object", ReturnType: "Navigator" },
				{ Name: "opener", Description: "Returns a reference to the window that created the window", OptionType: "Object", ReturnType: "Window" },
				{ Name: "outerHeight", Description: "Sets or returns the outer height of a window, including toolbars/scrollbars", OptionType: "Object", ReturnType: "Number" },
				{ Name: "outerWidth", Description: "Sets or returns the outer width of a window, including toolbars/scrollbars", OptionType: "Object", ReturnType: "Number" },
				{ Name: "pageXOffset", Description: "Returns the pixels the current document has been scrolled (horizontally) from the upper left corner of the window", OptionType: "Object", ReturnType: "Number" },
				{ Name: "pageYOffset", Description: "Returns the pixels the current document has been scrolled (vertically) from the upper left corner of the window", OptionType: "Object", ReturnType: "Number" },
				{ Name: "parent", Description: "Returns the parent window of the current window", OptionType: "Object", ReturnType: "Window" },
				{ Name: "screen", Description: "Returns the Screen object for the window", OptionType: "Object", ReturnType: "Screen" },
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
				
				{ Name: "close()", Description: "Closes the output stream previously opened with document.open()", OptionType: "Function" },
				{ Name: "getElementById(id)", Description: "Accesses the first element with the specified id", OptionType: "Function" },
				{ Name: "getElementsByName(name)", Description: "Accesses all elements with a specified name", OptionType: "Function" },
				{ Name: "getElementsByTagName(tagName)", Description: "Accesses all elements with a specified tagname", OptionType: "Function" },
				{ Name: "open()", Description: "Opens an output stream to collect the output from document.write() or document.writeln()", OptionType: "Function" },
				{ Name: "write(str)", Description: "Writes HTML expressions or JavaScript code to a document", OptionType: "Function" },
				{ Name: "writeln(str)", Description: "Same as write(), but adds a newline character after each statement", OptionType: "Function" }
			]
		},
		{
			Name: "Navigator",
			Options: [
				{ Name: "appCodeName", Description: "Returns the code name of the browser", OptionType: "Object", ReturnType: "String" },
				{ Name: "appName", Description: "Returns the name of the browser", OptionType: "Object", ReturnType: "String" },
				{ Name: "appVersion", Description: "Returns the version information of the browser", OptionType: "Object", ReturnType: "Number" },
				{ Name: "cookieEnabled", Description: "Determines whether cookies are enabled in the browser", OptionType: "Object", ReturnType: "Boolean" },
				{ Name: "platform", Description: "Returns for which platform the browser is compiled", OptionType: "Object", ReturnType: "String" },
				{ Name: "userAgent", Description: "Returns the user-agent header sent by the browser to the server", OptionType: "Object", ReturnType: "String" },
				
				{ Name: "javaEnabled()", Description: "Specifies whether or not the browser has Java enabled", OptionType: "Function", ReturnType: "Boolean" },
				{ Name: "taintEnabled()", Description: "Specifies whether or not the browser has data tainting enabled", OptionType: "Function", ReturnType: "Boolean" }
			]
		},
		{
			Name: "History",
			Options: [
				{ Name: "length", Description: "Returns the number of URLs in the history list", OptionType: "Object", ReturnType: "Number" },
				
				{ Name: "back()", Description: "Loads the previous URL in the history list", OptionType: "Function", ReturnType: "Object" },
				{ Name: "forward()", Description: "Loads the next URL in the history list", OptionType: "Function", ReturnType: "Object" },
				{ Name: "go()", Description: "Loads a specific URL from the history list", OptionType: "Function", ReturnType: "Object" }
			]
		},
		{
			Name: "Location",
			Options: [
				{ Name: "hash", Description: "Returns the anchor portion of a URL", OptionType: "Object", ReturnType: "String" },
				{ Name: "host", Description: "Returns the hostname and port of a URL", OptionType: "Object", ReturnType: "String" },
				{ Name: "hostname", Description: "Returns the hostname of a URL", OptionType: "Object", ReturnType: "String" },
				{ Name: "href", Description: "Returns the entire URL", OptionType: "Object", ReturnType: "String" },
				{ Name: "pathname", Description: "Returns the path name of a URL", OptionType: "Object", ReturnType: "String" },
				{ Name: "port", Description: "Returns the port number the server uses for a URL", OptionType: "Object", ReturnType: "Number" },
				{ Name: "protocol", Description: "Returns the protocol of a URL", OptionType: "Object", ReturnType: "String" },
				{ Name: "search", Description: "Returns the query portion of a URL", OptionType: "Object", ReturnType: "String" },
				
				{ Name: "assign()", Description: "Loads a new document", OptionType: "Function", ReturnType: "Object" },
				{ Name: "reload()", Description: "Reloads the current document", OptionType: "Function", ReturnType: "Object" },
				{ Name: "replace()", Description: "Replaces the current document with a new one", OptionType: "Function", ReturnType: "Object" }
			]
		},
		{
			Name: "Screen",
			Options: [
				{ Name: "availHeight", Description: "Returns the height of the screen (excluding the Windows Taskbar)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "availWidth", Description: "Returns the width of the screen (excluding the Windows Taskbar)", OptionType: "Object", ReturnType: "Number" },
				{ Name: "colorDepth", Description: "Returns the bit depth of the color palette for displaying images", OptionType: "Object", ReturnType: "Number" },
				{ Name: "height", Description: "Returns the total height of the screen", OptionType: "Object", ReturnType: "Number" },
				{ Name: "pixelDepth", Description: "Returns the color resolution (in bits per pixel) of the screen", OptionType: "Object", ReturnType: "Number" },
				{ Name: "width", Description: "Returns the total width of the screen", OptionType: "Object", ReturnType: "Number" }
			]
		}
	]
}