# NSIS Installer for VRChat Content Manager for Windows

## Make

```powershell
$Env:INSTALLER_DISPLAY_VERSION = "0.1.2-alpha.1"; $Env:INSTALLER_OLD_CLASSIC_VERSION = "0.1.2.0"; $Env:PathToBundle = "/path/to/bundle"; makensis installer.nsi
```

Output: `vrchat-content-manager-installer.exe`