function browse() {
	var textbox = document.getElementById("textclientpath");
	netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
	var nsIFilePicker = Components.interfaces.nsIFilePicker;
	var fp = Components.classes["@mozilla.org/filepicker;1"].createInstance(nsIFilePicker);
	fp.init(window, "Please locate PractiSES.exe", nsIFilePicker.modeOpen);
	fp.appendFilter("PractiSES Client", "PractiSES.exe");
	var res = fp.show();
	if (res == nsIFilePicker.returnOK) {
		textbox.thefile = fp.file;
		textbox.value = textbox.thefile.path;
	}
}