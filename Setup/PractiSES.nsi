;
; $Id: PractiSES.nsi 107 2007-04-25 18:51:58Z cbguder $
;

;--------------------------------
; Includes

	!include LogicLib.nsh
	!include MUI.nsh
	!include WordFunc.nsh

;--------------------------------
; General

	Name PractiSES
	OutFile practises-setup.exe

	InstallDir $PROGRAMFILES\PractiSES
	InstallDirRegKey HKLM "Software\PractiSES" "Install Directory"
	
	!define RELEASE "Debug"

;--------------------------------
; Interface Settings

	!define MUI_ABORTWARNING

	!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\orange-install.ico"
	!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\orange-uninstall.ico"

	!define MUI_HEADERIMAGE
	!define MUI_HEADERIMAGE_RIGHT
	!define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\orange-r.bmp"
	!define MUI_HEADERIMAGE_UNBITMAP "${NSISDIR}\Contrib\Graphics\Header\orange-uninstall-r.bmp"

	!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange.bmp"
	!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange-uninstall.bmp"

	!define MUI_FINISHPAGE_NOREBOOTSUPPORT
	!define MUI_FINISHPAGE_RUN $INSTDIR\ConfigurationWizard.exe
	!define MUI_FINISHPAGE_RUN_TEXT "Initialize keys now"

;--------------------------------
; Macros

	!insertmacro VersionCompare
	!insertmacro WordFind

;--------------------------------
; Pages

	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_LICENSE "..\GPL.txt"
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_INSTFILES
	!insertmacro MUI_PAGE_FINISH

	!insertmacro MUI_UNPAGE_WELCOME
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_COMPONENTS
	!insertmacro MUI_UNPAGE_INSTFILES
	;!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
; Languages

	!insertmacro MUI_LANGUAGE "English"
	
;--------------------------------
; Version Info

	VIProductVersion "1.0.0.0"
	VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "PractiSES"
	VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "PractiSES Team"
	VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "(c) 2006-2007 PractiSES Team"
	VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "PractiSES Client"
	VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.0.0.0"


;--------------------------------
; Install Types

	;InstType "Typical"
	;InstType "Custom"

;--------------------------------
; Sections

Section "Install"
	SectionIn RO

	;Set output path to the installation directory.
	SetOutPath $INSTDIR

	;Put file there
	File /oname=PractiSES.exe ..\Client\bin\${RELEASE}\Client.exe
	File ..\ConfigurationWizard\bin\${RELEASE}\ConfigurationWizard.exe
	File ..\Client\bin\${RELEASE}\Core.dll
	File ..\Client\bin\${RELEASE}\Crypto.dll
	File ..\Client\bin\${RELEASE}\IServer.dll
	File ..\Client\bin\${RELEASE}\Util.dll
	File ..\Extension\PractiSES.xpi
	File /oname=License.txt ..\GPL.txt
	
	;Write the installation path into the registry
	WriteRegStr HKLM SOFTWARE\PractiSES "Install Directory" "$INSTDIR"

	;Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PractiSES" "DisplayName" "PractiSES"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PractiSES" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PractiSES" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PractiSES" "NoRepair" 1
	WriteUninstaller "uninstall.exe"
SectionEnd

Section "un.PractiSES"
	SectionIn RO
	
	; Remove registry keys
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PractiSES"
	DeleteRegKey HKLM SOFTWARE\PractiSES

	; Remove files and uninstaller
	Delete $INSTDIR\PractiSES.exe
	Delete $INSTDIR\ConfigurationWizard.exe
	Delete $INSTDIR\Core.dll
	Delete $INSTDIR\Crypto.dll
	Delete $INSTDIR\IServer.dll
	Delete $INSTDIR\Util.dll
	Delete $INSTDIR\PractiSES.xpi
	Delete $INSTDIR\License.txt
	Delete $INSTDIR\uninstall.exe
	
	; Remove directories used
	RMDir $INSTDIR
SectionEnd

Section "un.User Keys"
	; Remove user profile files
    Delete $APPDATA\PractiSES\Client\answers.key
    Delete $APPDATA\PractiSES\Client\identity
    Delete $APPDATA\PractiSES\Client\private.key
	Delete $APPDATA\PractiSES\Client\server.key
    
    ; Remove user profile directory
    RMDir $APPDATA\PractiSES\Client
    RMDir $APPDATA\PractiSES
SectionEnd

Function .onInit
	Call GetThunderbirdVersion
	Pop $0
	
	${If} $0 == ""
		MessageBox MB_OK|MB_ICONSTOP "Thunderbird is not installed."
		Abort
	${Else}
		${VersionCompare} $0 "1.5.0.0" $1
		${If} $1 == 2
			MessageBox MB_OK|MB_ICONSTOP "Thunderbird v1.5 or newer is required. You have $0"
			Abort
		${EndIf}
	${EndIf}
FunctionEnd

Function GetThunderbirdVersion
	Push $0
	Push $1
	
	ReadRegStr $1 HKLM "Software\Mozilla\Mozilla Thunderbird" "CurrentVersion"
	${WordFind} $1 " " "+1" $0
	
	Pop $1
	Exch $0
FunctionEnd