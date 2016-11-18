@ECHO OFF
:Loop
IF "%1"=="OK" DO
   for %%i in (*.nupkg) do nuget push %%i -Source https://www.nuget.org/api/v2/package
   GOTO Continue
SHIFT
GOTO Loop
:Continue

