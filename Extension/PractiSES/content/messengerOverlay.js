var psesMsgObserver = {
	observe: function(aMsgFolder, aTopic, aData) {
		clearTimeout(this.timeoutID);
		document.getElementById("psesBox").collapsed = true;
		document.getElementById("psesBox2").collapsed = true;
		var message = practises.readMessage();
		if(message.indexOf("-----BEGIN PRACTISES SIGNED MESSAGE-----") == 0) {
			var hdr = messenger.msgHdrFromURI(aData);
			practises.verify(practises.stripEmail(hdr.mime2DecodedAuthor), null);
		} else if(message.indexOf("-----BEGIN PRACTISES MESSAGE-----") == 0) {
			document.getElementById("psesBox2").collapsed = false;
		}
	}
};

var ObserverService = Components.classes["@mozilla.org/observer-service;1"].getService(Components.interfaces.nsIObserverService);
ObserverService.addObserver(psesMsgObserver, "MsgMsgDisplayed", false);