@echo off
echo Opening the solution in Visual Studio...

rem Try to find Visual Studio installations
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" (
    for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
        if exist "%%i\Common7\IDE\devenv.exe" (
            echo Found Visual Studio: %%i
            "%%i\Common7\IDE\devenv.exe" Atom.VPN.Demo.sln
            goto :end
        )
    )
)

rem Check for older versions of Visual Studio
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" (
    echo Found Visual Studio 2015
    "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" Atom.VPN.Demo.sln
    goto :end
)

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" (
    echo Found Visual Studio 2013
    "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" Atom.VPN.Demo.sln
    goto :end
)

echo Visual Studio not found. Please open the solution manually.
pause

:end 