
; Inno Setup Script for Custom Layout Monitors
; Requires Inno Setup (https://jrsoftware.org/isdl.php)

#define MyAppName "Custom Layout Monitors"
#define MyAppVersion "1.0"
#define MyAppPublisher "Antigravity"
#define MyAppExeName "CustomLayoutMonitors.exe"
#define MyRegistryName "CustomLayoutMonitors"

[Setup]
AppId={{D3B3A5E1-7B8A-4F1E-9C2B-9D8F1A7B3E5C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=CustomLayoutMonitors_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "autostart"; Description: "Iniciar automáticamente con Windows"; GroupDescription: "Opciones adicionales:"; Flags: checkedonce

[Files]
Source: "d:\DESARROLLO\ANTIGRAVITY\Custom Layout Monitors\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
; Configure auto-start in registry if the task is selected
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyRegistryName}"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue; Tasks: autostart

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
