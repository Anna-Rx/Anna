@echo off
call "%VS100COMNTOOLS%..\..\VC\vcvarsall.bat"
msbuild %~dp0default.build /t:All /nologo
pause