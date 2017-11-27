@ECHO OFF

SET PROJECT_NAME=WebApi

SET MSDEPLOY_PATH=C:\Program Files (x86)\IIS\Microsoft Web Deploy V3

SET PUBLISH_PATH=C:\Temp\PublishTemp

SET WORKSPACE_PATH=%~1

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar

cd "%WORKSPACE_PATH%"

cmd /c dotnet publish "%WORKSPACE_PATH%\src\Sofco.%PROJECT_NAME%" -c release --output "%PUBLISH_PATH%\Sofco.%PROJECT_NAME%"

"%MSDEPLOY_PATH%\msdeploy.exe" -source:manifest="%WORKSPACE_PATH%\build\Azgap01wp.%PROJECT_NAME%.SourceManifest.xml" -dest:package="%PUBLISH_PATH%\%PROJECT_NAME%.zip" -verb:sync -retryAttempts:20 -disablerule:BackupRule

"%MSDEPLOY_PATH%\msdeploy.exe" -source:package="%PUBLISH_PATH%\%PROJECT_NAME%.zip" -dest:manifest="%WORKSPACE_PATH%\build\Azgap01wp.%PROJECT_NAME%.DestinationManifest.xml",ComputerName='https://azgap01wp:8172/msdeploy.axd?site=Sofco.%PROJECT_NAME%',UserName='AZGAP01WP\SofcoWebDeploy',Password='Hellspawn!01',IncludeAcls='False',AuthType='Basic' -verb:sync -enablerule:AppOffline -enableRule:DoNotDeleteRule -allowUntrusted
