@echo off
echo Компиляция Unmask...
cd /d "%~dp0"

echo.
echo [1/2] Компиляция GUI версии...
dotnet build Unmask.csproj --configuration Release
if %ERRORLEVEL% neq 0 (
    echo Ошибка при компиляции GUI версии!
    pause
    exit /b 1
)

echo.
echo [2/2] Компиляция консольной версии...
dotnet build Unmask.Console.csproj --configuration Release
if %ERRORLEVEL% neq 0 (
    echo Ошибка при компиляции консольной версии!
    pause
    exit /b 1
)

echo.
echo ✅ Компиляция завершена успешно!
echo.
echo Файлы созданы:
echo - GUI версия: bin\Release\net472\Unmask.exe
echo - Консольная версия: bin\Release\net472\Unmask.Console.exe
echo.
pause 