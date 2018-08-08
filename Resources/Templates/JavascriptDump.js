function isArray(testObject) {
    return testObject && !(testObject.propertyIsEnumerable('length')) && typeof testObject === 'object' && typeof testObject.length === 'number';
}

function Dump(obj, depth, message) {
    var output = '<table>';

    if (depth == undefined)
        depth = 1;

    if (message != undefined) {
        output +=
			"<table class='headingpresenter'>" +
				"<tr>" +
					"<th class='headingpresenter'>" +
						"<p>" + message + "</p>" +
					"</th>" +
				"</tr>" +
				"<tr>" +
					"<td class='headingpresenter'>";
    }

    output +=
		"<table>" +
			"<tr>" +
				"<td class='typeheader' colspan='" + (isArray(obj) ? "1" : "2") + "'>" +
					"<a href='' class='typeheader' onclick='return toggle();'>" +
						"<span class='typeglyph' id='t9ud'>5</span>" +
							(isArray(obj)
							    ? "Array (" + obj.length + " items)"
							    : obj.toString()) +
					"</a>" +
				"</td>" +
			"</tr>";

    //	output +=
    //			"<tr>" + 
    //				"<td colspan='2' class='summary'>Summary information</td>" +
    //			"</tr>";

    for (property in obj) {
        if (property == 'Dump')
            continue;

        output +=
			"<tr>";

        if (!isArray(obj)) {
            output += "<th class='member' title='" + typeof (obj[property]) + "'>" + property + "</th>";
        }

        output +=
				"<td>" +
					"<p>";

        if (obj[property] == null) {
            output += "null";
        } else if (depth > 5) {
            output += "too deep";
        } else if (typeof (obj[property]) == 'object') {
            output += Dump(obj[property], depth + 1);
        } else {
            output += obj[property];
        }

        output +=
					"</p>" +
				"</td>" +
			"</tr>"
    }

    output +=
		"</table>";

    if (message != undefined) {
        output +=
					"</td>" +
				"</tr>" +
			"</table>";
    }

    return output;
};

Object.prototype.Dump = function(message) {
    $(Dump(this, 1, message)).appendTo("#dumpOutput");
};