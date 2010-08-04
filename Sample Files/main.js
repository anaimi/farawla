$(function() {
	// hidden details and hidden detail owners
	$(".hidden-detail-owner").live('mouseenter', function() {
			$(".hidden-detail", this).show();
		}).live('mouseleave', function() {
			$(".hidden-detail", this).hide();
		});

	// water mark
	initializeWaterMarkedInputs();
});

function initializeWaterMarkedInputs() {
	// set the value from title
	$(".water-marked").each
	(function() {
		if ($(this).val() == "") {
			$(this).val($(this).attr("title"));
			$(this).addClass("gray-foreground");
		}
	});

	// set event handlers
	$(".water-marked").focus
	(function() {
		if ($(this).val() == $(this).attr("title")) {
			$(this).val("");
			$(this).removeClass("water-marked");
		}
	}).blur
	(function() {
		if ($(this).val() == "") {
			$(this).val($(this).attr("title"));
			$(this).addClass("water-marked");
		}
	});
}

// templating (http://www.west-wind.com/weblog/posts/509108.aspx)
var _tmplCache = {}
template = function(str, data) {
	var err = "";
	try {
		var func = _tmplCache[str];
		if (!func) {
			var strFunc =
            "var p=[],print=function(){p.push.apply(p,arguments);};" +
                        "with(obj){p.push('" +
            str.replace(/[\r\t\n]/g, " ")
               .replace(/'(?=[^#]*#>)/g, "\t")
               .split("'").join("\\'")
               .split("\t").join("'")
               .replace(/<#=(.+?)#>/g, "',$1,'")
               .split("<#").join("');")
               .split("#>").join("p.push('")
               + "');}return p.join('');";

			//alert(strFunc);
			func = new Function("obj", strFunc);
			_tmplCache[str] = func;
		}
		return func(data);
	} catch (e) { err = e.message; }
	return "< # ERROR: " + err.htmlEncode() + " # >";
}

// dialog
jQuery.fn.dialog = function() {
	$.fancybox(this, {
		'scrolling': 'no',
		'titleShow': false,
		'onStart': function() { this.show(); },
		'onComplete': function() { $.fancybox.resize(); },
		'onClosed': function() { this.hide(); }
	});
	
	return this;
}

// isBlank
jQuery.fn.isBlank = function() {
	if ($.trim(this.val()) == "")
		return true;
	if (this.val() == this.attr("title"))
		return true;
	return false;
}

// pluralize
Number.prototype.pluralize = function(single, multiple) {
	if (this == 0)
		return "no " + multiple;
	if (this == 1)
		return "one " + single;
	return this + " " + multiple;
}

// postGo (to HTTP POST without a form or AJAX)
function postGo(url, params) {
	var $form = $("<form>").attr("method", "post").attr("action", url);
	
	$.each(params, function(name, value) {
		$("<input type='hidden'>").attr("name", name).attr("value", value).appendTo($form);
	});
	
	$form.appendTo("body");
	$form.submit();
}

// pagination
function paginate(container, nextPageFunctionName, page, perPage, total) {
	container.html("");
	var pageCount = Math.ceil(total / perPage);
	
	if (pageCount == 1)
		return;

	container.append("<ul class='paginator'>");
	for (var i = 0; i < pageCount; i++)
		container.append("<li><a href='javascript:" + nextPageFunctionName + "(" + (i + 1) + ");'>" + (i + 1) + "</a></li>");
	container.append("</ul>");
}