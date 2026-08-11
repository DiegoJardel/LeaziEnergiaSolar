#define MyAppName "Controle de Comissões Leazi"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Leazi Energia Solar"
#define MyAppExeName "LeaziEnergiaSolar.Wpf.exe"
#define MyAppIcon "..\LeaziEnergiaSolar\src\LeaziEnergiaSolar.Wpf\Assets\Icons\leazi-app.ico"

[Setup]
AppId={{B14CF17A-EAC8-42DB-9A2C-473685D974C8}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppComments=Sistema de Controle de Vendas e Comissões
DefaultDirName={localappdata}\Programs\Controle de Comissões Leazi
DefaultGroupName=Controle de Comissões Leazi
OutputDir=output
OutputBaseFilename=Setup_ControleComissoes_Leazi_{#MyAppVersion}
SetupIconFile={#MyAppIcon}
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
CloseApplications=yes
RestartApplications=no
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Sistema de Controle de Vendas e Comissões
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; GroupDescription: "Atalhos adicionais:"

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Controle de Comissões Leazi"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\Controle de Comissões Leazi"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir Controle de Comissões Leazi"; WorkingDir: "{app}"; Flags: nowait postinstall skipifsilent
