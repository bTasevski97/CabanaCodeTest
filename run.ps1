# run.ps1 - Windows Entrypoint

$ErrorActionPreference = "Stop"

$runArgs = $args

if ($runArgs.Count -eq 0) {
    Write-Host "No map argument provided. Select a map file from below:" -ForegroundColor Yellow
    
    $files = @(Get-ChildItem -Filter "*.ascii" -File)
    
    if ($files.Count -eq 0) {
        Write-Error "No .ascii files found in the current directory."
        exit 1
    } elseif ($files.Count -eq 1) {
        $mapFile = $files[0].FullName
        Write-Host "Only one map found. Auto-selecting: $mapFile" -ForegroundColor Green
    } else {
        for ($i = 0; $i -lt $files.Count; $i++) {
            Write-Host "[$i] $($files[$i].Name)"
        }
        
        $selection = Read-Host "Enter number"
        
        if ([string]::IsNullOrWhiteSpace($selection) -or $selection -notmatch '^\d+$' -or [int]$selection -ge $files.Count -or [int]$selection -lt 0) {
            Write-Error "Invalid selection."
            exit 1
        }
        
        $mapFile = $files[[int]$selection].FullName
        Write-Host "Selected map: $mapFile" -ForegroundColor Green
    }
    $runArgs = @("--map", $mapFile)
    # Auto-detect bookings.json in current directory if not explicitly provided
    if (Test-Path "bookings.json") {
        $bookingsFile = (Get-Item "bookings.json").FullName
        $runArgs += "--bookings"
        $runArgs += $bookingsFile
    }

} else {
    # Validate and convert arguments early
    $newArgs = @()
    for ($i = 0; $i -lt $runArgs.Count; $i++) {
        $arg = $runArgs[$i]
        if ($arg -match '^--.*\.ascii$') {
            Write-Error "Error: Detected argument '$arg' which looks like a file but starts with '--'."
            Write-Host "Format should be: .\run.ps1 --map filename.ascii" -ForegroundColor Yellow
            exit 1
        }
        
        if ($arg -eq '--map' -or $arg -eq '--bookings') {
            if ($i + 1 -ge $runArgs.Count) {
                Write-Error "Error: $arg specified but no file name provided."
                exit 1
            }
            $filePath = $runArgs[$i+1]
            if (-not (Test-Path $filePath)) {
                Write-Error "Error: File '$filePath' does not exist."
                exit 1
            }
            $absPath = (Get-Item -LiteralPath $filePath).FullName
            $newArgs += $arg
            $newArgs += $absPath
            $i++
        } else {
            $newArgs += $arg
        }
    }
    $runArgs = $newArgs
}

# Ensure dependencies are installed
if (-not (Test-Path "src/ClientApp/node_modules")) {
    Write-Host "--- Installing Frontend Dependencies ---" -ForegroundColor Cyan
    Set-Location src/ClientApp
    npm install
    Set-Location ../..
}

Write-Host "--- Launching Resort Map ---" -ForegroundColor Cyan
# Open browser in background once the server is ready
Start-Job -ScriptBlock { npx -y wait-on http://localhost:5173; Start-Process "http://localhost:5173" } | Out-Null

# Forward all arguments to the app using '--'
# SpaProxy will automatically start the frontend dev server (npm run dev)
dotnet run --project src/ResortMap/ResortMap.csproj -- $runArgs

