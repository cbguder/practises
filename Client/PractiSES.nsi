; $Id$

;--------------------------------
; Include Modern UI header

  !include "MUI.nsh"

;--------------------------------
; General

  Name "PractiSES"
  OutFile "practises-setup.exe"

  InstallDir $PROGRAMFILES\PractiSES
  InstallDirRegKey HKLM "Software\PractiSES" "Install_Dir"

;--------------------------------
; Interface Settings

  !define MUI_ABORTWARNING

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

  ;Set output path to the installation directory.
  SetOutPath $INSTDIR

  ;Put file there
  File /oname=PractiSES.exe "bin\Debug\Client.exe"
  File /oname=License.txt "..\GPL.txt"
  
  ;Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\PractiSES "Install_Dir" "$INSTDIR"

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
	Delete $INSTDIR\License.txt
	Delete $INSTDIR\uninstall.exe

	; Remove directories used
	RMDir "$INSTDIR"
SectionEnd