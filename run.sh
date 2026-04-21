#!/bin/bash

# Exit on error
set -e

if [ "$#" -eq 0 ]; then
    echo "No map argument provided. Select a map file from below:"
    
    # Collect files safely
    shopt -s nullglob
    files=(*.ascii)
    shopt -u nullglob
    
    if [ ${#files[@]} -eq 0 ]; then
        echo "No .ascii files found in the current directory."
        exit 1
    elif [ ${#files[@]} -eq 1 ]; then
        MAP_FILE="${files[0]}"
        echo "Only one map found. Auto-selecting: $MAP_FILE"
    else
        for i in "${!files[@]}"; do
            echo "[$i] ${files[$i]}"
        done
        
        read -p "Enter number: " selection
        
        if ! [[ "$selection" =~ ^[0-9]+$ ]] || [ "$selection" -ge "${#files[@]}" ] || [ "$selection" -lt 0 ]; then
            echo "Invalid selection."
            exit 1
        fi
        
        MAP_FILE="${files[$selection]}"
        echo "Selected map: $MAP_FILE"
    fi
    
    # Convert to absolute path
    ABS_MAP="$(pwd)/$MAP_FILE"
    echo "Selected map: $ABS_MAP"
    
    # Auto-detect bookings.json in current directory if not explicitly provided
    if [ -f "bookings.json" ]; then
        ABS_BOOKINGS="$(pwd)/bookings.json"
        set -- "--map" "$ABS_MAP" "--bookings" "$ABS_BOOKINGS"
    else
        set -- "--map" "$ABS_MAP"
    fi

else
    # Validate and convert arguments early
    new_args=()
    while [[ $# -gt 0 ]]; do
        case "$1" in
            --map)
                if [ -n "$2" ] && [ -f "$2" ]; then
                    ABS_PATH="$(cd "$(dirname "$2")" && pwd)/$(basename "$2")"
                    new_args+=("--map" "$ABS_PATH")
                    shift 2
                else
                    echo "Error: Map file '$2' does not exist."
                    exit 1
                fi
                ;;
            --bookings)
                if [ -n "$2" ] && [ -f "$2" ]; then
                    ABS_PATH="$(cd "$(dirname "$2")" && pwd)/$(basename "$2")"
                    new_args+=("--bookings" "$ABS_PATH")
                    shift 2
                else
                    echo "Error: Bookings file '$2' does not exist."
                    exit 1
                fi
                ;;
            --*.ascii)
                echo "Error: Detected argument '$1' which looks like a file but starts with '--'."
                echo "Format should be: ./run.sh --map filename.ascii"
                exit 1
                ;;
            *)
                new_args+=("$1")
                shift
                ;;
        esac
    done
    set -- "${new_args[@]}"
fi

# Ensure dependencies are installed
if [ ! -d "src/ClientApp/node_modules" ]; then
    echo "--- Installing Frontend Dependencies ---"
    cd src/ClientApp
    npm install
    cd ../..
fi

open_url() {
    local url=$1
    if command -v open >/dev/null 2>&1; then
        open "$url"
    elif command -v xdg-open >/dev/null 2>&1; then
        xdg-open "$url"
    elif [[ "$OSTYPE" == "msys" || "$OSTYPE" == "cygwin" ]]; then
        start "$url"
    else
        echo "--- Application ready at $url ---"
    fi
}

echo "--- Launching Resort Map ---"
# Open browser in background once the server is ready
(npx -y wait-on http://localhost:5173 && open_url http://localhost:5173) &


# Forward all arguments to the app using '--'
# SpaProxy will automatically start the frontend dev server (npm run dev)
dotnet run --project src/ResortMap/ResortMap.csproj -- "$@"

