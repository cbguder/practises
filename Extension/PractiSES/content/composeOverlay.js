var prefs = Components.classes["@mozilla.org/preferences-service;1"].getService(Components.interfaces.nsIPrefService);
prefs = prefs.getBranch("extensions.practises.");

var clientpath			= prefs.getCharPref("clientpath");
var host				= prefs.getCharPref("server");
const CHARSET			= "UTF-8";
const REPLACEMENTCHAR	= Components.interfaces.nsIConverterInputStream.DEFAULT_REPLACEMENT_CHARACTER;

var practises = {
	onLoad: function() {
		this.initialized = true;
		this.strings = document.getElementById("practises-strings");
	},
	prompt: function(title, message) {
		var prompts = Components.classes["@mozilla.org/embedcomp/prompt-service;1"].getService(Components.interfaces.nsIPromptService);
		var input = {value:""};
		var check = {value:false};
		var dialogResult = prompts.promptPassword(window, title, message, input, null, check);

		if(!dialogResult || input.value == "") {
			return null;
		}

		return input.value;
	},
	callPractises: function(command, argument) {
		var domndEditor = document.getElementById("content-frame");
		var htmlEditor = domndEditor.getHTMLEditor(domndEditor.contentWindow);
		htmlEditor = htmlEditor.QueryInterface(Components.interfaces.nsIPlaintextEditor);

		var tmp_file = Components.classes["@mozilla.org/file/directory_service;1"].getService(Components.interfaces.nsIProperties).get("TmpD", Components.interfaces.nsIFile);
		tmp_file.append("message.tmp");
		tmp_file.createUnique(Components.interfaces.nsIFile.NORMAL_FILE_TYPE, 0664);

		var ostream = Components.classes["@mozilla.org/network/file-output-stream;1"].createInstance(Components.interfaces.nsIFileOutputStream);
		ostream.init(tmp_file, 0x02 | 0x08 | 0x20, 0664, 0);
			var os = Components.classes["@mozilla.org/intl/converter-output-stream;1"].createInstance(Components.interfaces.nsIConverterOutputStream);
			os.init(ostream, CHARSET, 1024, REPLACEMENTCHAR);
			os.writeString(htmlEditor.outputToString("text/plain", 0));
			os.close();
		ostream.close();

		var gpg_file = Components.classes["@mozilla.org/file/local;1"].createInstance(Components.interfaces.nsILocalFile);
		var gpg_process = Components.classes["@mozilla.org/process/util;1"].createInstance(Components.interfaces.nsIProcess);
		var gpg_args;

		if(command == "-s" || command == "-d" || command == "-c") {
			gpg_args = [command, "-H", host, "-p", argument, tmp_file.path];
		} else if(command == "-e") {
			gpg_args = [command, "-H", host, "-r", argument, tmp_file.path];
		}

		gpg_file.initWithPath(clientpath);
		gpg_process.init(gpg_file);
		gpg_process.run(true, gpg_args, gpg_args.length);

		var signed_file = Components.classes["@mozilla.org/file/local;1"].createInstance(Components.interfaces.nsILocalFile);
		signed_file.initWithPath(tmp_file.path + ".pses");
		var istream = Components.classes["@mozilla.org/network/file-input-stream;1"].createInstance(Components.interfaces.nsIFileInputStream);
		istream.init(signed_file, 0x01, 0, 0);
		istream.QueryInterface(Components.interfaces.nsIInputStream);
		var is = Components.classes["@mozilla.org/intl/converter-input-stream;1"].createInstance(Components.interfaces.nsIConverterInputStream);
		is.init(istream, CHARSET, 1024, REPLACEMENTCHAR);
		var data = "";
		var str = {};
		while(is.readString(1024, str) != 0) {
			data += str.value;
		};
		is.close();
		istream.close();

		htmlEditor.selectAll();
		htmlEditor.insertText(data);

		tmp_file.remove(false);
		signed_file.remove(false);
	},
	encrypt: function() {
		var node = document.getElementById("addressCol2#1");
		var recipient;
		if(node.value.indexOf("<") == -1) {
			recipient = node.value;
		} else {
			var start = node.value.indexOf("<");
			var end = node.value.indexOf(">");
			recipient = node.value.substring(start + 1, end);
		}
		practises.callPractises("-e", recipient);
	},
	sign: function() {
		var passphrase = practises.prompt("PractiSES", "Enter passphrase:");
		practises.callPractises("-s", passphrase);

		var bucket = document.getElementById("attachmentBucket");
		var urls = new Array();
		var i;
		
		for(i = 0; i < bucket.childNodes.length; i++) {
			url = bucket.childNodes[i].attachment.url;
			url = url.substring(8, url.length);
			url = url.replace(/\//g, "\\");
			url = decodeURI(url);
			urls[i] = url;
		}
		
		for(i = 0; i < urls.length; i++) {
			practises.addDetachedSignature(passphrase, urls[i]);
		}
	},
	addDetachedSignature: function(passphrase, url) {
		var filename = url.substring(url.lastIndexOf("\\") + 1, url.length);

//		var tmp_file = Components.classes["@mozilla.org/file/directory_service;1"].getService(Components.interfaces.nsIProperties).get("TmpD", Components.interfaces.nsIFile);
//		tmp_file.append(filename + ".pses");

		var tmp_file = "D:\\" + filename + ".pses";

		var gpg_file = Components.classes["@mozilla.org/file/local;1"].createInstance(Components.interfaces.nsILocalFile);
		var gpg_process = Components.classes["@mozilla.org/process/util;1"].createInstance(Components.interfaces.nsIProcess);
		var gpg_args = ["--sign-detached", "-H", host, "-p", passphrase, "-O", tmp_file, url];
		gpg_file.initWithPath(clientpath);
		gpg_process.init(gpg_file);
		gpg_process.run(true, gpg_args, gpg_args.length);
		
		var attachment = Components.classes["@mozilla.org/messengercompose/attachment;1"].createInstance(Components.interfaces.nsIMsgAttachment);
		var finalurl = tmp_file.replace(/\\/g, "/");
		finalurl = "file:///" + encodeURI(finalurl);
		attachment.url = finalurl;

		if(attachment && attachment.url) {
		    var bucket = document.getElementById("attachmentBucket");
			var item = document.createElement("listitem");

			if (!attachment.name)
				attachment.name = filename + ".pses";

			item.setAttribute("label", attachment.name);    //use for display only
			item.attachment = attachment;   //full attachment object stored here
			try {
				item.setAttribute("tooltiptext", decodeURI(attachment.url));
			} catch(e) {
				item.setAttribute("tooltiptext", attachment.url);
			}
			item.setAttribute("class", "listitem-iconic");
			item.setAttribute("image", "moz-icon:" + attachment.url);
			bucket.appendChild(item);
		}
  	}
};

function onComposerSendMessage()
{
	var signRadio = document.getElementById("practises-sign");
	var encryptRadio = document.getElementById("practises-encrypt");
	var signAndEncryptRadio = document.getElementById("practises-signAndEncrypt");

	var CHECKED = signRadio.accessible.STATE_CHECKED;

	var sign = false;
	var encrypt = false;
	
	if(signAndEncryptRadio.accessible.finalState & CHECKED) {
		sign = true;
		encrypt = true;
	} else if(signRadio.accessible.finalState & CHECKED) {
		sign = true;
	} else if(encryptRadio.accessible.finalState & CHECKED) {
		encrypt = true;
	}

	if(sign) {
//		practises.sign();
	}

	if(encrypt) {
		practises.encrypt();
	}
};

window.addEventListener("load", function(e) { practises.onLoad(e); }, false);
window.addEventListener('compose-send-message', onComposerSendMessage, true);
