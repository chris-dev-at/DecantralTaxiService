@echo off

REM Change the directory to the project directory
cd AndersTaxi

REM Build and start the Docker containers
docker-compose up -d --build

REM Tell user what he is waiting for
echo "Waiting for Blazor to start..."

REM Wait for Blazor to start
timeout /t 15 >nul

REM Open the webpage in the default browser on Windows
start http://localhost:8000