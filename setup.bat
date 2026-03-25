@echo off
REM Script de Inicialização - Banking Core Microservice (Windows)
REM Suporta SQLite e SQL Server

setlocal enabledelayedexpansion

echo.
echo ================================
echo Banking Core Microservice Setup
echo ================================
echo.

REM Verificar .NET SDK
echo Verificando .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERRO: .NET SDK não encontrado!
    echo Instale em: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [OK] .NET SDK encontrado: %DOTNET_VERSION%
echo.

REM Escolher banco de dados
echo Escolha o banco de dados:
echo 1) SQLite (padrao - recomendado para desenvolvimento)
echo 2) SQL Server (requer instalacao)
set /p DB_CHOICE="Selecione (1 ou 2): "

if "%DB_CHOICE%"=="2" (
    set DB_TYPE=sqlserver
    echo Usando SQL Server
    set DATABASE_TYPE=sqlserver
    
    set /p CONNECTION_STR="Digite a connection string (pressione Enter para usar localdb): "
    if "!CONNECTION_STR!"=="" (
        set CONNECTION_STR=Server=(localdb)\mssqllocaldb;Database=BankingCore;Integrated Security=true;
    )
) else (
    set DB_TYPE=sqlite
    set CONNECTION_STR=Data Source=banking.db
    echo Usando SQLite
)

echo.
echo Limpando construcoes anteriores...
dotnet clean >nul 2>&1
echo [OK] Limpeza concluida
echo.

echo Restaurando dependencias...
dotnet restore
echo [OK] Dependencias restauradas
echo.

echo Compilando projeto...
dotnet build
echo [OK] Projeto compilado com sucesso
echo.

if "%DB_TYPE%"=="sqlserver" (
    echo Aplicando migrations do Entity Framework...
    dotnet ef database update --project src\Core.Infrastructure 2>nul
)

if "%DB_TYPE%"=="sqlite" (
    if exist "banking.db" (
        set /p RECREATE="Arquivo 'banking.db' existe. Deseja recriar? (s/n): "
        if "!RECREATE!"=="s" (
            del /f banking.db
            echo [OK] Banco de dados anterior removido
        )
    )
)

echo.
echo ================================
echo [OK] Setup Concluido!
echo ================================
echo.

echo Para iniciar a API, execute:
echo.
echo   dotnet run --project src\Core.API\Core.API.csproj
echo.
echo Depois acesse:
echo   http://localhost:5000/swagger/index.html
echo.

set /p RUN_NOW="Deseja executar a API agora? (s/n): "
if "%RUN_NOW%"=="s" (
    echo.
    echo Iniciando API...
    echo.
    dotnet run --project src\Core.API\Core.API.csproj
)

pause
