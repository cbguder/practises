var prefs = Components.classes["@mozilla.org/preferences-service;1"].getService(Components.interfaces.nsIPrefService);
prefs = prefs.getBranch("extensions.practises.");

var clientpath			= prefs.getCharPref("clientpath");
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

		if(!dialogResult || input.value == "")
		{
			return null;
		}

		return input.value;
	},
	callPractises: function(command, argument) {
		var messagePane = document.getElementById("messagepane");
		var msgPaneDocChildren = messagePane.contentDocument.childNodes;
		var ndHTML = msgPaneDocChildren.item(0);
		var ndBODY = ndHTML.childNodes.item(1);

		var tmp_file = Components.classes["@mozilla.org/file/directory_service;1"].getService(Components.interfaces.nsIProperties).get("TmpD", Components.interfaces.nsIFile);
		tmp_file.append("message.tmp");
		tmp_file.createUnique(Components.interfaces.nsIFile.NORMAL_FILE_TYPE, 0664);

		var ostream = Components.classes["@mozilla.org/network/file-output-stream;1"].createInstance(Components.interfaces.nsIFileOutputStream);
		ostream.init(tmp_file, 0x02 | 0x08 | 0x20, 0664, 0);
			var os = Components.classes["@mozilla.org/intl/converter-output-stream;1"].createInstance(Components.interfaces.nsIConverterOutputStream);
			os.init(ostream, CHARSET, 1024, REPLACEMENTCHAR);
			os.writeString(ndBODY.textContent);
			os.close();
		ostream.close();

		var gpg_file = Components.classes["@mozilla.org/file/local;1"].createInstance(Components.interfaces.nsILocalFile);
		var gpg_process = Components.classes["@mozilla.org/process/util;1"].createInstance(Components.interfaces.nsIProcess);
		var gpg_args;

		if(command == "-s" || command == "-d" || command == "--finalize-initialize" || command == "--finalize-update" || command == "--finalize-remove")
		{
			gpg_args = [command, "-p", argument, tmp_file.path];
		}
		else if(command == "-e")
		{
			gpg_args = [command, "-r", argument, tmp_file.path];
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
			while(is.readString(1024, str) != 0)
			{
				data += str.value;
			};
			is.close();
		istream.close();

		ndBODY.textContent = data;

		tmp_file.remove(false);
		signed_file.remove(false);
	},
	finalizeInitialize: function(e) {
		var passphrase = practises.prompt("PractiSES", "Enter passphrase:");
		practises.callPractises("--finalize-initialize", passphrase);
	},
	finalizeUpdate: function(e) {
		var passphrase = practises.prompt("PractiSES", "Enter passphrase:");
		practises.callPractises("--finalize-update", passphrase);
	},
	finalizeRemove: function(e) {
		var passphrase = practises.prompt("PractiSES", "Enter passphrase:");
		practises.callPractises("--finalize-remove", passphrase);
	},
	decrypt: function(e) {
		var passphrase = practises.prompt("PractiSES", "Enter passphrase:");
		practises.callPractises("-d", passphrase);
	}
};

window.addEventListener("load", function(e) { practises.onLoad(e); }, false);
