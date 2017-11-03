SET PROJECT_NAME=Sofco.ClientWeb

SET PUBLISH_PATH=C:\Temp\PublishTemp\%PROJECT_NAME%

SET WORKSPACE_PATH=%~1

SET DEPLOY_PATH=%~2

IF [%1] == [] SET WORKSPACE_PATH=C:\Projects\Sofcoar
IF [%2] == [] SET DEPLOY_PATH=\\AZSOF01WT\Sofco.ClientWeb

cd "%WORKSPACE_PATH%\src\%PROJECT_NAME%"

cmd /c npm install

cmd /c DEL /F /Q "%PUBLISH_PATH%"
cmd /c FOR /D %%p IN ("%PUBLISH_PATH%\*.*") DO rmdir "%%p" /s /q

cmd /c ng build --prod --env=azsof01wt -op "%PUBLISH_PATH%"

cmd /c DEL /F /Q "%DEPLOY_PATH%"
cmd /c FOR /D %%p IN ("%DEPLOY_PATH%\*.*") DO rmdir "%%p" /s /q

cmd /c XCOPY /S "%PUBLISH_PATH%" "%DEPLOY_PATH%"

cmd /c DEL /F /Q "%PUBLISH_PATH%"
cmd /c FOR /D %%p IN ("%PUBLISH_PATH%\*.*") DO rmdir "%%p" /s /q
