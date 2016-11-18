@ECHO OFF
:Loop
IF "%1"=="" GOTO Continue
   nuget pack DotQueue.Client.nuspec -version %1
   nuget pack DotQueue.HostLib.nuspec -version %1
   nuget pack DotQueue.Persistence.RavenDB -version %1
SHIFT
GOTO Loop
:Continue
