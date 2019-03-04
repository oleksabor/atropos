; SilentInstall silent
; Icon "ddmitov.ico"

; DEPENDENCIES:
; Version plug-in: http://nsis.sourceforge.net/Version_plug-in
; XtInfoPlugin plug-in: http://nsis.sourceforge.net/XtInfoPlugin_plug-in
; Simple Service plug-in: http://nsis.sourceforge.net/NSIS_Simple_Service_Plugin

!define registryKeyName "atropos"
!define regKeyStr "Software\${registryKeyName}"
!define msUninstallKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\" 
!define /file Version "..\src\versionFile"

; The file to write
OutFile "..\bin\atropos.${Version}.exe"

;--------------------------------
; The default installation directory
InstallDir $PROGRAMFILES\atropos 

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM $regKeyStr "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin


var TempResult

Function .onInit

	# detect Windows XP
	Version::IsWindowsXP
	# get result
	Pop $TempResult
	# check result
	StrCmp $TempResult "1" IsUserAdmin NotWindowsXP

	NotWindowsXP:
	MessageBox MB_OK "This program requires Windows XP."
	Abort

FunctionEnd


;--------------------------------
var conpath
;write file and concatenate its name 
; file name must contain path part if it is placed to the $OUTDIR sub directory
!macro FileReg path fn
File "${path}${fn}"

StrCpy $conpath "${fn}|$conpath"
!macroend


;--------------------------------
; Pages
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "English"
;--------------------------------
ShowInstDetails show

Section "Main"

	WriteRegStr HKLM ${regKeyStr} "Install_Dir" "$INSTDIR"
	

	; Write the uninstall keys for Windows
	WriteRegStr HKLM "${msUninstallKey}${registryKeyName}" "DisplayName" "Atropos (uninstall)"
	WriteRegStr HKLM "${msUninstallKey}\${registryKeyName}" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "${msUninstallKey}\${registryKeyName}" "NoModify" 1
	WriteRegDWORD HKLM "${msUninstallKey}\${registryKeyName}" "NoRepair" 1
	
	;D:\work\atropos\src\server\bin\Release\
	!define serverBin "..\src\server\bin\Release\"
	!define clientBin "..\src\client\bin\Release\"
	
	SetOutPath "$INSTDIR\server"
	!insertmacro FileReg ${serverBin} "atroposServer.exe"
	!insertmacro FileReg ${serverBin} "atroposServer.pdb"
	!insertmacro FileReg ${serverBin} "atroposServer.exe.config"
	!insertmacro FileReg ${serverBin} "Common.dll"
	!insertmacro FileReg ${serverBin} "Common.pdb"
	!insertmacro FileReg ${serverBin} "linq2db.dll"
	!insertmacro FileReg ${serverBin} "linq2db.pdb"
	!insertmacro FileReg ${serverBin} "linq2db.xml"
	!insertmacro FileReg ${serverBin} "NLog.config"
	!insertmacro FileReg ${serverBin} "NLog.dll"
	!insertmacro FileReg ${serverBin} "StructureMap.dll"
	!insertmacro FileReg ${serverBin} "System.Data.SQLite.dll"
	!insertmacro FileReg ${serverBin} "System.Data.SQLite.dll.config"
	!insertmacro FileReg ${serverBin} "Topshelf.dll"
	!insertmacro FileReg ${serverBin} "Topshelf.LibLog.dll"
	!insertmacro FileReg ${serverBin} "WcfHosting.dll"
	!insertmacro FileReg ${serverBin} "WcfHosting.pdb"
	SetOutPath "$INSTDIR\server\x86"
	!insertmacro FileReg "${serverBin}\x86" "SQLite.Interop.dll"
	SetOutPath "$INSTDIR\server\x64"
	!insertmacro FileReg "${serverBin}\x64" "SQLite.Interop.dll"

	SetOutPath "$INSTDIR\client"
	!insertmacro FileReg ${clientBin} "atroposServer.exe"
	!insertmacro FileReg ${clientBin} "client.exe"
	!insertmacro FileReg ${clientBin} "client.pdb"
	!insertmacro FileReg ${clientBin} "client.exe.config"
	!insertmacro FileReg ${clientBin} "Common.dll"
	!insertmacro FileReg ${clientBin} "Common.pdb"
	!insertmacro FileReg ${clientBin} "Hardcodet.Wpf.TaskbarNotification.dll"
	!insertmacro FileReg ${clientBin} "Hardcodet.Wpf.TaskbarNotification.pdb"
	!insertmacro FileReg ${clientBin} "Hardcodet.Wpf.TaskbarNotification.xml"
	!insertmacro FileReg ${clientBin} "NLog.config"
	!insertmacro FileReg ${clientBin} "NLog.dll"
	!insertmacro FileReg ${clientBin} "WcfHosting.dll"
	!insertmacro FileReg ${clientBin} "WcfHosting.pdb"	
	
	WriteRegStr HKLM ${regKeyStr} "installed_files" $conpath

	WriteUninstaller "uninstall.exe"

	;shortcuts
	CreateDirectory "${programsMenu}"
	CreateShortCut "${programsMenu}\Uninstall.lnk" $INSTDIR\uninstall.exe
	
	Exec '"$INSTDIR\server\atroposServer.exe install --localsystem'
	Exec 'net start atropos.server'


SectionEnd

;--------------------------------
; Uninstaller
Section "Uninstall"
; read installed files and delete them  
	ReadRegStr $0 HKLM ${regKeyStr} "installed_files"
	nsArray::Split fileList "$0" "|" /noempty
	
	; Remove registry keys
	DeleteRegKey HKLM "${msUninstallKey}${registryKeyName}"
	DeleteRegKey HKLM ${regKeyStr}
;	Call "un.GetMyDocs"
;	StrCpy $myDocs $0
	
	; Remove files and uninstaller
	Delete "$INSTDIR\uninstall.exe"
	${ForEachIn} fileList $R0 $R1
		IfFileExists "$INSTDIR\$R1" 0 +2
			Delete "$INSTDIR\$R1"
		IfFileExists "$R1" 0
			Delete "$R1"
	${Next}

	RMDir "$INSTDIR\server\x86"
	RMDir "$INSTDIR\server\x64"
	RMDir "$INSTDIR\server"
	RMDir "$INSTDIR\client"
	RMDir $INSTDIR
 
	RMDir /r "${programsMenu}"
SectionEnd
