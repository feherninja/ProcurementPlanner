; Installer that unpacks the Velopack portable build into a folder of the user's choice.
; After installation the app updates itself (Update.exe replaces the "current" folder);
; the database lives in "data", which neither updates nor uninstall touch.
;
; Build: ISCC.exe /DAppVersion=1.2.3 /DSourceDir=<extracted Portable.zip> /DOutputDir=<dir> ProcurementPlanner.iss

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif
#ifndef SourceDir
  #error SourceDir must point to the extracted ProcurementPlanner-win-Portable.zip
#endif
; The shortcuts point at the Velopack launcher, which vpk names after --packTitle
#if !FileExists(SourceDir + "\ProcurementPlanner.exe")
  #error ProcurementPlanner.exe missing from SourceDir; was vpk pack run with --packTitle ProcurementPlanner?
#endif
#ifndef OutputDir
  #define OutputDir "."
#endif

[Setup]
AppId={{1F6DCB96-FC58-4CFD-A229-A0E916AAC190}
AppName=Beszerzéstervező
AppVersion={#AppVersion}
AppPublisher=Beszerzéstervező
DefaultDirName=C:\Beszerzestervezo
DefaultGroupName=Beszerzéstervező
DisableProgramGroupPage=yes
DirExistsWarning=no
; Updates run without admin rights, so the app must live in a folder the user can write to
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\ProcurementPlanner.exe
UninstallDisplayName=Beszerzéstervező
OutputDir={#OutputDir}
OutputBaseFilename=Beszerzestervezo-Setup
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"

[CustomMessages]
hungarian.ProgramFilesNotAllowed=A Program Files mappa nem használható, mert oda a program nem tudja telepíteni a frissítéseit. Válassz másik mappát, például C:\Beszerzestervezo.
hungarian.FirstStartHint=Első indításkor a program felajánlja a korábbi verzió adatbázisának (delivery.db) betöltését.

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Dirs]
Name: "{app}\data"; Flags: uninsneveruninstall

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\Beszerzéstervező"; Filename: "{app}\ProcurementPlanner.exe"
Name: "{autodesktop}\Beszerzéstervező"; Filename: "{app}\ProcurementPlanner.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\ProcurementPlanner.exe"; Description: "{cm:LaunchProgram,Beszerzéstervező}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Files written by the updater after installation are not tracked by Inno, remove them explicitly.
; The data folder (database) is kept on purpose.
Type: filesandordirs; Name: "{app}\current"
Type: filesandordirs; Name: "{app}\packages"
Type: files; Name: "{app}\*.log"

[Code]
function IsUnderFolder(Path, Folder: String): Boolean;
begin
  Result := (Folder <> '') and (Pos(Lowercase(AddBackslash(Folder)), Lowercase(AddBackslash(Path))) = 1);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Dir: String;
begin
  Result := True;
  if CurPageID = wpSelectDir then
  begin
    Dir := WizardDirValue;
    if IsUnderFolder(Dir, ExpandConstant('{commonpf32}')) or IsUnderFolder(Dir, ExpandConstant('{commonpf64}')) then
    begin
      MsgBox(CustomMessage('ProgramFilesNotAllowed'), mbError, MB_OK);
      Result := False;
    end;
  end;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpFinished then
    WizardForm.FinishedLabel.Caption := WizardForm.FinishedLabel.Caption + #13#10#13#10 +
      CustomMessage('FirstStartHint');
end;
