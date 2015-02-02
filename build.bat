@echo off

:checkMsBuild
set msbuild=
for /D %%a in (%SYSTEMROOT%\Microsoft.NET\Framework\v4.0*) do set msbuild=%%a\MSBuild.exe
if not defined msbuild (
	echo error: can't find MSBuild.exe. Is .NET Framework installed?
	exit /B 2
)

:checkNuget
for %%a in (NuGet.exe) do (set nugetPath=%%~$PATH:a)
if not defined nugetPath (
	echo error: can't find NuGet.exe in your path.
	exit /B 3
)

:setDefaults
if not defined Configuration set Configuration=Release
if not defined EnableNuGetPackageRestore set EnableNuGetPackageRestore=true
if not defined PackageSources set PackageSources=https://www.nuget.org/api/v2/

set solutionDir=%~dp0%
if not "%cd%\"=="%solutionDir%" (
  echo change to directory "%solutionDir%"
  cd /d "%solutionDir%"
)

:packageRestore
NuGet.exe restore -source "%PackageSources%"

:build
echo %DATE% %TIME% > .build\build.stamp.cache
%msbuild% %* /p:CustomBeforeMicrosoftCommonTargets="%solutionDir%.build\empty.targets"
