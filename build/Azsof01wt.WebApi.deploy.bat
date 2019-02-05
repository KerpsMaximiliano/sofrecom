@ECHO OFF

SET PROJECT_NAME=WebApi

SET MSDEPLOY_PATH=C:\Program Files (x86)\IIS\Microsoft Web Deploy V3

SET PUBLISH_PATH=C:\Temp\PublishTemp

SET WORKSPACE_PATH=%~1

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar

cd "%WORKSPACE_PATH%"

cmd /c dotnet publish "%WORKSPACE_PATH%\src\Sofco.%PROJECT_NAME%" -c release --output "%PUBLISH_PATH%\Sofco.%PROJECT_NAME%"

"%MSDEPLOY_PATH%\msdeploy.exe" -source:manifest="%WORKSPACE_PATH%\build\Azsof01wt.%PROJECT_NAME%.SourceManifest.xml" -dest:package="%PUBLISH_PATH%\%PROJECT_NAME%.zip" -verb:sync -retryAttempts:20 -disablerule:BackupRule

"%MSDEPLOY_PATH%\msdeploy.exe" -verb:sync -source:package="%PUBLISH_PATH%\%PROJECT_NAME%.zip" -dest: contentPath='azsof01wt-web', ComputerName="https://azsof01wt-web.publish.azurewebsites.windows.net:443/msdeploy.axd?site=azsof01wt-web", UserName='AZSOF01WD\SofcoWebDeploy', Password='Hellspawn!01', AuthType='Basic'

