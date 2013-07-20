@echo off
if defined VS100COMNTOOLS (call "%VS100COMNTOOLS%..\..\VC\vcvarsall.bat")
if defined VS110COMNTOOLS (call "%VS110COMNTOOLS%..\..\VC\vcvarsall.bat")

msbuild %~dp0default.build /t:All /nologo
pause