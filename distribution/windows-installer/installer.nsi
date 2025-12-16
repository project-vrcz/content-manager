Unicode True

!define MUI_BGCOLOR "SYSCLR:Window"
!define MUI_TEXTCOLOR "SYSCLR:WindowText"

!include MUI2.nsh
!include nsDialogs.nsh

# Constant Definitions
!define ProductName "VRChat Content Manager"
!define Publisher "Misaka-L"
!define HomePageUrl "https://github.com/project-vrcz/content-manager"
!define ReleasesUrl "https://github.com/project-vrcz/content-manager/releases"

!define DisplayVersion "$%INSTALLER_DISPLAY_VERSION%"
!define OldClassicVersion "$%INSTALLER_OLD_CLASSIC_VERSION%"

!define PathToBundle "$%INSTALLER_PATH_TO_BUNDLE%"

!define AppExecutable "VRChatContentManager.App.exe"

!define /date InstallDate "%Y%m%d"

!define AppDataPath "$LOCALAPPDATA\vrchat-content-manager-81b7bca3\"

!define INSTDIR_REG_ROOT HKLM
!define INSTDIR_REG_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${ProductName}"

!define START_MENU_PAGE_ID "Application"

; Advanced Uninstall Log
; https://nsis.sourceforge.io/Advanced_Uninstall_Log_NSIS_Header
!include .\include\AdvUninstLog.nsh
!insertmacro UNATTENDED_UNINSTALL

; Attributes
Name "${ProductName}"
OutFile "vrchat-content-manager-installer.exe"
RequestExecutionLevel admin
AllowSkipFiles off
SetCompressor /SOLID lzma

InstallDir "$PROGRAMFILES64\${ProductName}"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT HKLM
!define MUI_STARTMENUPAGE_REGISTRY_KEY $INSTDIR_REG_KEY
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"

ShowInstDetails show
ShowUninstDetails show

ManifestSupportedOS Win10
ManifestDPIAware true
ManifestDPIAwareness PerMonitorV2

; Version Information
BrandingText "${Publisher}"
VIFileVersion "${OldClassicVersion}"
VIProductVersion "${OldClassicVersion}"
VIAddVersionKey "FileVersion" "${OldClassicVersion}"
VIAddVersionKey "ProductVersion" "${DisplayVersion}"
VIAddVersionKey "ProductName" "${ProductName}"
VIAddVersionKey "CompanyName" "${Publisher}"
VIAddVersionKey "FileDescription" "${ProductName} Installer"
VIAddVersionKey "LegalCopyright" "Copyright (c) ${Publisher}"

; UI
!define MUI_LANGDLL_ALLLANGUAGES
!define MUI_ABORTWARNING
!define MUI_ICON "icon.ico"
!define MUI_UNICON "icon.ico"
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE

Var RemoveAppDataConfirmPageCheckbox_State

; Install Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "../../LICENSE"
!insertmacro MUI_PAGE_DIRECTORY

var StartMenuFolder
!insertmacro "MUI_PAGE_STARTMENU" $START_MENU_PAGE_ID $StartMenuFolder

!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Uninstall Pages
!insertmacro MUI_UNPAGE_CONFIRM
UninstPage custom un.onRemoveAppDataPage un.onRemoveAppDataPageLeave
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; UI L10N
!insertmacro MUI_LANGUAGE English
!insertmacro MUI_LANGUAGE "SimpChinese"

; Install Sections
Section "Install Files"
    SetOutPath $INSTDIR
    !insertmacro UNINSTALL.LOG_OPEN_INSTALL

    File "${PathToBundle}\*.*"

    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "DisplayName" "${ProductName}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "DisplayVersion" "${DisplayVersion}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "DisplayIcon" "$INSTDIR\${AppExecutable}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "Publisher" "${Publisher}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "HelpLink" "${HomePageUrl}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "URLInfoAbout" "${HomePageUrl}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "URLUpdateInfo" "${ReleasesUrl}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "Comments" "A more efficient way to publish your VRChat contents."
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "InstallLocation" "$INSTDIR"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "UninstallString" "${UNINST_EXE}"
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "QuietUninstallString" '"${UNINST_EXE}" /S'
    WriteRegDWORD "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "NoModify" 1
    WriteRegDWORD "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "NoRepair" 1
    WriteRegStr "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "InstallDate" "${InstallDate}"

    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
    WriteRegDWORD "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}" "EstimatedSize" "$0"

    !insertmacro UNINSTALL.LOG_CLOSE_INSTALL
SectionEnd

Section "-Create Shoartcuts"
    !insertmacro MUI_STARTMENU_WRITE_BEGIN $START_MENU_PAGE_ID
      CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
      CreateShortCut "$SMPROGRAMS\$StartMenuFolder\${ProductName}.lnk" "$INSTDIR\${AppExecutable}"
      CreateShortCut "$DESKTOP\${ProductName}.lnk" "$INSTDIR\${AppExecutable}"
    !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

; Uninstall Sections
Section "uninstall"
    !insertmacro UNINSTALL.LOG_UNINSTALL "$INSTDIR"
    !insertmacro UNINSTALL.LOG_END_UNINSTALL

    !insertmacro MUI_STARTMENU_GETFOLDER $START_MENU_PAGE_ID $StartMenuFolder
    RMDir /r "$SMPROGRAMS\$StartMenuFolder\"
    Delete "$DESKTOP\${ProductName}.lnk"

    ${If} $RemoveAppDataConfirmPageCheckbox_State == ${BST_CHECKED}
        RMDir /r "${AppDataPath}"
    ${EndIf}

    DeleteRegKey "${INSTDIR_REG_ROOT}" "${INSTDIR_REG_KEY}"
SectionEnd

; onInit
Function .onInit
    System::Call 'kernel32::CreateMutex(p 0, i 0, t "${ProductName}InstallerMutex") p .r1 ?e'
    Pop $R0

    StrCmp $R0 0 +3
        MessageBox MB_OK|MB_ICONEXCLAMATION "The installer is already running."
        Abort

    !insertmacro MUI_LANGDLL_DISPLAY
    !insertmacro UNINSTALL.LOG_PREPARE_INSTALL
FunctionEnd

Function .onInstSuccess
    !insertmacro UNINSTALL.LOG_UPDATE_INSTALL
FunctionEnd

Function un.onInit
    System::Call 'kernel32::CreateMutex(p 0, i 0, t "${ProductName}InstallerMutex") p .r1 ?e'
    Pop $R0

    StrCmp $R0 0 +3
        MessageBox MB_OK|MB_ICONEXCLAMATION "The installer is already running."
        Abort

    !insertmacro MUI_LANGDLL_DISPLAY
    !insertmacro UNINSTALL.LOG_BEGIN_UNINSTALL
FunctionEnd

Var RemoveAppDataConfirmPageDialog
Var RemoveAppDataConfirmPageLabel
Var RemoveAppDataConfirmPageCheckbox

Function un.onRemoveAppDataPage
    nsDialogs::Create 1018
    Pop $RemoveAppDataConfirmPageDialog

    ${If} $RemoveAppDataConfirmPageDialog == error
        Abort
    ${EndIf}

    ${NSD_CreateLabel} 0 0 100% 12u "It includes all user settings, login information."
	Pop $RemoveAppDataConfirmPageLabel

    ${NSD_CreateCheckbox} 0 30u 100% 10u "&Yes, remove all application data."
	Pop $RemoveAppDataConfirmPageCheckbox

    !insertmacro MUI_HEADER_TEXT "Remove Application Data" "Do you want to remove all application data associated with ${ProductName}?"

    nsDialogs::Show
FunctionEnd

Function un.onRemoveAppDataPageLeave
    ${NSD_GetState} $RemoveAppDataConfirmPageCheckbox $RemoveAppDataConfirmPageCheckbox_State
FunctionEnd