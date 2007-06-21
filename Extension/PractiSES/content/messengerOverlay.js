var psesMsgObserver = {
	observe: function(aMsgFolder, aTopic, aData) {
		document.getElementById("psesBox").collapsed = true;
		var message = practises.readMessage();
		if(message.indexOf("-----BEGIN PRACTISES SIGNED MESSAGE-----") == 0) {
			var hdr = messenger.msgHdrFromURI(aData);
			practises.verify(practises.stripEmail(hdr.mime2DecodedAuthor));
		}
	}
};

var ObserverService = Components.classes["@mozilla.org/observer-service;1"].getService(Components.interfaces.nsIObserverService);
ObserverService.addObserver(psesMsgObserver, "MsgMsgDisplayed", false);