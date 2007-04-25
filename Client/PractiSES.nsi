; $Id$

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

;--------------------------------
; Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
; Macros

  !insertmacro VersionCompare
  !insertmacro WordFind

;--------------------------------
; Pages

  !insertmacro MUI_PAGE_LICENSE "..\GPL.txt"
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES

  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
; Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Sections

Section "PractiSES"
  SectionIn RO
  
  Call CompareThunderbirdVersion
  Pop $0
  DetailPrint $0
  
  ${If} $0 == "not found"
    MessageBox MB_OK|MB_ICONSTOP ".NET runtime library is not installed."
    Abort
  ${EndIf}
  
  ;Set output path to the installation directory.
  SetOutPath $INSTDIR

  ;Put file there
  File /oname=PractiSES.exe bin\Debug\Client.exe
  File bin\Debug\Core.dll
  File bin\Debug\Crypto.dll
  File bin\Debug\Util.dll
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

Section "Uninstall"
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\PractiSES"
  DeleteRegKey HKLM SOFTWARE\PractiSES

  ; Remove files and uninstaller
  Delete $INSTDIR\PractiSES.exe
  Delete $INSTDIR\Core.dll
  Delete $INSTDIR\Crypto.dll
  Delete $INSTDIR\Util.dll
  Delete $INSTDIR\License.txt
  Delete $INSTDIR\uninstall.exe

  ; Remove directories used
  RMDir "$INSTDIR"
SectionEnd

Function CompareThunderbirdVersion
  Push $0
  Push $1
  
  ReadRegStr $0 HKLM "Software\Mozilla\Mozilla Thunderbird" "CurrentVersion"
  ${WordFind} $0 " " "+1" $1
  ${VersionCompare} $1 "1.5.0.0" $0
  
  Pop $1
  Exch $0
FunctionEnd