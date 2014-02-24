@echo off
if "%~1" == "" (
    echo "Output name missing"
    exit /b 1
)
..\Tools\X86-Windows\llc.exe -filetype=obj -o=%1.o ..\%1.ll
if %errorlevel% neq 0 exit /b %errorlevel%
..\Tools\X86-Windows\clang++.exe -c -o Externals.o Externals.cpp
if %errorlevel% neq 0 exit /b %errorlevel%
link -nologo -defaultlib:libcmt -subsystem:console -out:%1.exe %1.o Externals.o
