@echo off
echo Компиляция LostMyMisoSoup...
cd /d "%~dp0"

echo.
echo [1/2] Компиляция GUI версии...
dotnet build LostMyMisoSoup.csproj --configuration Release
if %ERRORLEVEL% neq 0 (
    echo Ошибка при компиляции GUI версии!
    pause
    exit /b 1
)

echo.
echo [2/2] Компиляция консольной версии...
dotnet build LostMyMisoSoup.Console.csproj --configuration Release
if %ERRORLEVEL% neq 0 (
    echo Ошибка при компиляции консольной версии!
    pause
    exit /b 1
)

echo.
echo ✅ Компиляция завершена успешно!
echo.
echo Файлы созданы:
echo - GUI версия: bin\Release\net472\LostMyMisoSoup.exe
echo - Консольная версия: bin\Release\net472\LostMyMisoSoup.Console.exe
echo.
pause 