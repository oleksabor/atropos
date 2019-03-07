; DEPENDENCIES:
; Version plug-in: http://nsis.sourceforge.net/Version_plug-in
; XtInfoPlugin plug-in: http://nsis.sourceforge.net/XtInfoPlugin_plug-in
; Simple Service plug-in: http://nsis.sourceforge.net/NSIS_Simple_Service_Plugin
; nsArray plugin https://nsis.sourceforge.io/Arrays_in_NSIS
; https://nsis.sourceforge.io/LogicLib

!include "MUI.nsh"
!include 'LogicLib.nsh'
!include nsArray.nsh
!include "FileFunc.nsh"


!define registryKeyName "atropos"
!define regKeyStr "Software\${registryKeyName}"
!define msUninstallKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\" 
!define /file Version "..\src\versionFile"


!define programsMenu "$SMPROGRAMS\Atropos"
; The file to write
OutFile "..\bin\atropos.${Version}.exe"
;--------------------------------
; The default installation directory
InstallDir $PROGRAMFILES\atropos 

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\atropos" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

Name "Atropos"
Icon "..\src\client\Resources\scissors.ico"

;--------------------------------
var conpath ; used to store installed file names
;write file and concatenate its name 
; file name must contain path part if it is placed to the $OUTDIR sub directory
!macro FileReg path fileName installPath
File "${path}${fileName}"

StrCpy $conpath "${installPath}\${fileName}|$conpath"
!macroend
!macro FileRegS path fileName 
!insertmacro FileReg ${path} ${filename} "server"
!macroend
!macro FileRegC path fileName 
!insertmacro FileReg ${path} ${filename} "client"
!macroend

var dateTime 

!macro FileLog line

  !insertmacro GetTime
  ${GetTime} "" "L" $0 $1 $2 $3 $4 $5 $6
  StrCpy $dateTime "$2$1$0-$4$5$6"

FileOpen $4 "$INSTDIR\setup.log" a
FileSeek $4 0 END
FileWrite $4 "$dateTime ${line}"
FileWrite $4 "$\r$\n" ; we write an extra line
FileClose $4 ; and close the file

DetailPrint "${line}"

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

!define serverBin "..\src\server\bin\Release\"
!define clientBin "..\src\client\bin\Release\"
!define serviceName "atropos.server"
var uninstRK
var serviceSet

Function .onInit

FunctionEnd

!macro ServiceExists name
StrCpy $R0 '"$SYSDIR\sc.exe" query ${name}'
nsExec::ExecToStack '$R0'
;Pop $R1  # contains return code
;Pop $R2  # contains output
!macroend
!macro ServiceStop name
StrCpy $R0 '"$SYSDIR\sc.exe" stop ${name}'
nsExec::ExecToStack '$R0'
;Pop $R1  # contains return code
;Pop $R2  # contains output
!macroend
!macro ServiceStart name
StrCpy $R0 '"$SYSDIR\sc.exe" start ${name}'
nsExec::ExecToStack '$R0'
;Pop $R1  # contains return code
;Pop $R2  # contains output
!macroend
!macro ServiceDelete name
StrCpy $R0 '"$SYSDIR\sc.exe" delete ${name}'
nsExec::ExecToStack '$R0'
;Pop $R1  # contains return code
;Pop $R2  # contains output
!macroend
Section "Main"

	WriteRegStr HKLM ${regKeyStr} "Install_Dir" "$INSTDIR"
	!insertmacro FileLog "instaling to the $INSTDIR (saved to the ${regKeyStr})"

	StrCpy $uninstRK "${msUninstallKey}${registryKeyName}"
	
	; Write the uninstall keys for Windows
	WriteRegStr HKLM $uninstRK "DisplayName" "Atropos (uninstall)"
	WriteRegStr HKLM $uninstRK "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM $uninstRK "NoModify" 1
	WriteRegDWORD HKLM $uninstRK "NoRepair" 1
	
	!insertmacro FileLog "uninstall info was saved to the $uninstRK"
	
	!insertmacro ServiceExists ${serviceName}
	Pop $R1 ; returns an errorcode if the service doesn´t exists (<>0)/service exists (0)
	StrCpy $serviceSet $R1
	
	!insertmacro FileLog "service was found $serviceSet $R1"	
	
	${If} $serviceSet = 0
		!insertmacro FileLog "service is stopping ${serviceName}"
		SimpleSC::StopService ${serviceName} 1 30
		Pop $0
		!insertmacro FileLog "${serviceName} service stop result $0"
		
		SimpleSC::RemoveService ${serviceName}
		Pop $0
		!insertmacro FileLog "${serviceName} service uninstall result $0"
	${Endif}
  
	SetOutPath "$INSTDIR\server\"
	!insertmacro FileRegS "${serverBin}" "atroposServer.exe" 
	!insertmacro FileRegS ${serverBin} "atroposServer.pdb"
	!insertmacro FileRegS ${serverBin} "atroposServer.exe.config"
	!insertmacro FileRegS ${serverBin} "Common.dll"
	!insertmacro FileRegS ${serverBin} "Common.pdb"
	!insertmacro FileRegS ${serverBin} "Google.Protobuf.dll"
	!insertmacro FileRegS ${serverBin} "Google.Protobuf.pdb"
	!insertmacro FileRegS ${serverBin} "Grpc.Core.dll"
	!insertmacro FileRegS ${serverBin} "Grpc.Core.pdb"
	!insertmacro FileRegS ${serverBin} "Grpc.Core.Api.dll"
	!insertmacro FileRegS ${serverBin} "Grpc.Core.Api.pdb"
	!insertmacro FileRegS ${serverBin} "grpc_csharp_ext.x64.dll"
	!insertmacro FileRegS ${serverBin} "grpc_csharp_ext.x86.dll"
	!insertmacro FileRegS ${serverBin} "libgrpc_csharp_ext.x64.dylib"
	!insertmacro FileRegS ${serverBin} "libgrpc_csharp_ext.x64.so"
	!insertmacro FileRegS ${serverBin} "libgrpc_csharp_ext.x86.dylib"
	!insertmacro FileRegS ${serverBin} "libgrpc_csharp_ext.x86.so"
	!insertmacro FileRegS ${serverBin} "Nerdle.AutoConfig.dll"
	!insertmacro FileRegS ${serverBin} "linq2db.dll"
	!insertmacro FileRegS ${serverBin} "linq2db.pdb"
	!insertmacro FileRegS ${serverBin} "linq2db.xml"
	!insertmacro FileRegS ${serverBin} "NLog.config"
	!insertmacro FileRegS ${serverBin} "NLog.dll"
	!insertmacro FileRegS ${serverBin} "StructureMap.dll"
	!insertmacro FileRegS ${serverBin} "System.Data.SQLite.dll"
	!insertmacro FileRegS ${serverBin} "System.Data.SQLite.dll.config"
	!insertmacro FileRegS ${serverBin} "Topshelf.dll"
	!insertmacro FileRegS ${serverBin} "Topshelf.LibLog.dll"
	!insertmacro FileRegS ${serverBin} "System.Interactive.Async.dll"
	SetOutPath "$INSTDIR\server\x86"
	!insertmacro FileRegS "${serverBin}" "x86\SQLite.Interop.dll"
	SetOutPath "$INSTDIR\server\x64"
	!insertmacro FileRegS "${serverBin}" "x64\SQLite.Interop.dll"

	SetOutPath "$INSTDIR\client\"
	!insertmacro FileRegC ${clientBin} "client.exe"
	!insertmacro FileRegC ${clientBin} "client.pdb"
	!insertmacro FileRegC ${clientBin} "client.exe.config"
	!insertmacro FileRegC ${clientBin} "Common.dll"
	!insertmacro FileRegC ${clientBin} "Common.pdb"
	!insertmacro FileRegC ${clientBin} "Hardcodet.Wpf.TaskbarNotification.dll"
	!insertmacro FileRegC ${clientBin} "Hardcodet.Wpf.TaskbarNotification.pdb"
	!insertmacro FileRegC ${clientBin} "Hardcodet.Wpf.TaskbarNotification.xml"
	!insertmacro FileRegC ${clientBin} "NLog.config"
	!insertmacro FileRegC ${clientBin} "NLog.dll"
	!insertmacro FileRegC ${clientBin} "Google.Protobuf.dll"
	!insertmacro FileRegC ${clientBin} "Google.Protobuf.pdb"
	!insertmacro FileRegC ${clientBin} "Grpc.Core.dll"
	!insertmacro FileRegC ${clientBin} "Grpc.Core.pdb"
	!insertmacro FileRegC ${clientBin} "Grpc.Core.Api.dll"
	!insertmacro FileRegC ${clientBin} "Grpc.Core.Api.pdb"
	!insertmacro FileRegC ${clientBin} "grpc_csharp_ext.x64.dll"
	!insertmacro FileRegC ${clientBin} "grpc_csharp_ext.x86.dll"
	!insertmacro FileRegC ${clientBin} "libgrpc_csharp_ext.x64.dylib"
	!insertmacro FileRegC ${clientBin} "libgrpc_csharp_ext.x64.so"
	!insertmacro FileRegC ${clientBin} "libgrpc_csharp_ext.x86.dylib"
	!insertmacro FileRegC ${clientBin} "libgrpc_csharp_ext.x86.so"
	!insertmacro FileRegC ${clientBin} "Nerdle.AutoConfig.dll"
	!insertmacro FileRegC ${clientBin} "System.Interactive.Async.dll"

	WriteRegStr HKLM ${regKeyStr} "installed_files" $conpath
	
	!insertmacro FileLog "files were written to the ${regKeyStr}"
	!insertmacro FileLog $conpath

	WriteUninstaller "uninstall.exe"

	;shortcuts
	CreateDirectory "${programsMenu}"
	CreateShortCut "${programsMenu}\Uninstall.lnk" $INSTDIR\uninstall.exe
	CreateShortCut "${programsMenu}\client.lnk" $INSTDIR\client\client.exe 
	
	DetailPrint "installing ${serviceName}"
	nsExec::ExecToStack '"$INSTDIR\server\atroposServer.exe" install --localsystem'
	Pop $0
	Pop $1
	!insertmacro FileLog "service was installed, $0 $1"

	!insertmacro ServiceStart ${serviceName}
	Pop $0
	Pop $1
	!insertmacro FileLog "service start result ${serviceName} $0 $1"

SectionEnd

;--------------------------------
; Uninstaller
Section "Uninstall"
; read installed files and delete them  

	!insertmacro ServiceExists ${serviceName}
	Pop $R1 ; returns an errorcode if the service doesn´t exists (<>0)/service exists (0)
	StrCpy $serviceSet $R1
	
	!insertmacro FileLog "service was found $serviceSet $R1"	
	
	${If} $serviceSet = 0
		!insertmacro FileLog "${serviceName} service was installed, stopping"
		SimpleSC::StopService "${serviceName}" 1 30
		
		SimpleSC::RemoveService "${serviceName}"
		Pop $0
		!insertmacro FileLog "${serviceName} service uninstall result $0"
	${Endif}

	ReadRegStr $0 HKLM ${regKeyStr} "installed_files"
	nsArray::Split fileList "$0" "|" /noempty
	${ForEachIn} fileList $R0 $R1
		IfFileExists "$INSTDIR\$R1" 0 +2
			Delete "$INSTDIR\$R1"
		IfFileExists "$R1" 0
			Delete "$R1"
	${Next}

	; Remove registry keys
	DeleteRegKey HKLM "${msUninstallKey}${registryKeyName}"
	DeleteRegKey HKLM ${regKeyStr}

	; Remove files and uninstaller
	Delete "$INSTDIR\uninstall.exe"
	
	RMDir "$INSTDIR\server\x86"
	RMDir "$INSTDIR\server\x64"
	RMDir "$INSTDIR\server"
	RMDir "$INSTDIR\client"
	RMDir $INSTDIR
 
	RMDir /r "${programsMenu}"
SectionEnd
