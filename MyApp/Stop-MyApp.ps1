# Stop-MyApp.ps1
# Quick script to stop MyApp.exe process before building

Write-Host "Stopping MyApp.exe processes..." -ForegroundColor Yellow

# Kill all MyApp.exe processes
$processes = Get-Process -Name "MyApp" -ErrorAction SilentlyContinue

if ($processes) {
    $processes | ForEach-Object {
        Write-Host "Killing process: MyApp.exe (PID: $($_.Id))" -ForegroundColor Red
        Stop-Process -Id $_.Id -Force
    }
    Write-Host "? All MyApp.exe processes stopped!" -ForegroundColor Green
} else {
    Write-Host "? No MyApp.exe processes running." -ForegroundColor Green
}

# Wait a moment for file handles to be released
Start-Sleep -Milliseconds 500

Write-Host "Ready to build!" -ForegroundColor Cyan
