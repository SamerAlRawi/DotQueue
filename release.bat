@ECHO OFF

MSBuild.exe DotQueue.sln /t:Build /p:Configuration=Release
if /I "%ERRORLEVEL%" NEQ "0" (
   echo MSBUILD Task failed %ERRORLEVEL%
   exit /b %ERRORLEVEL%
)

nunit3-console.exe ".\DotQueue.HostLib.Tests\bin\Release\DotQueue.HostLib.Tests.dll" --output=result.xml --where "cat != no_ci"
if /I "%ERRORLEVEL%" NEQ "0" (
   echo NUNIT Task failed %ERRORLEVEL%
   exit /b %ERRORLEVEL%
)

nunit3-console.exe ".\DotQueue.Client.Tests\bin\Release\DotQueue.Client.Tests.dll" --output=result.xml --where "cat != no_ci"
if /I "%ERRORLEVEL%" NEQ "0" (
   echo NUNIT Task failed %ERRORLEVEL%
   exit /b %ERRORLEVEL%
)

:Loop
IF "%1"=="" GOTO Continue
   nuget pack DotQueue.Client.nuspec -version %1
   nuget pack DotQueue.HostLib.nuspec -version %1
   nuget pack DotQueue.Persistence.RavenDB.nuspec -version %1
   nuget pack DotQueue.Persistence.SQLite.nuspec -version %1
SHIFT
GOTO Loop
:Continue
