@ECHO OFF

SET WORKSPACE_PATH=%~1

SET SONAR_SCAN_PATH=C:\Install.Program\SonarQubeScanner\netcoreapp2\SonarScanner.MSBuild.dll

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar

cd "%WORKSPACE_PATH%"

cmd /c dotnet "%SONAR_SCAN_PATH%" begin /k:"Gaps-ApiJob:develop" /d:sonar.host.url="http://azsof01wd:9100" /d:sonar.login="admin" /d:sonar.password="admin"
cmd /c dotnet build
cmd /c dotnet "%SONAR_SCAN_PATH%" end /d:sonar.login="admin" /d:sonar.password="admin"
