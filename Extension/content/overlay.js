const GPG				= "C:\\Program\ Files\\GNU\\GnuPG\\gpg.exe";
const CHARSET			= "UTF-8";
const REPLACEMENTCHAR	= Components.interfaces.nsIConverterInputStream.DEFAULT_REPLACEMENT_CHARACTER;

var practises = {
	onLoad: function() {
		this.initialized = true;
		this.strings = document.getElementById("practises-strings");
	},
	onMenuItemCommand: function(e) {
		practises.clearSignMessage();
	},
	onToolbarButtonCommand: function(e) {
		practises.onMenuItemCommand(e);
	},
	clearSignMessage: function() {
		var prompts = Components.classes["@mozilla.org/embedcomp/prompt-service;1"].getService(Components.interfaces.nsIPromptService);
		var input = {value:""};
		var check = {value:false};
		var dialogResult = prompts.promptPassword(window, 'GPG Requires Passphrase', 'Enter Passphrase:', input, null, check);
		var passphrase = input.value

		if(!dialogResult || passphrase == "")
		{
			return;
		}

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
		var gpg_args = ["--clearsign", "--charset", "utf8", "--passphrase", passphrase, tmp_file.path];

		gpg_file.initWithPath(GPG);
		gpg_process.init(gpg_file);
		gpg_process.run(true, gpg_args, gpg_args.length);

		var signed_file = Components.classes["@mozilla.org/file/local;1"].createInstance(Components.interfaces.nsILocalFile);
		signed_file.initWithPath(tmp_file.path + ".asc");
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

		htmlEditor.selectAll();
		htmlEditor.insertText(data);

		tmp_file.remove(false);
		signed_file.remove(false);
	}
};

window.addEventListener("load", function(e) { practises.onLoad(e); }, false);
