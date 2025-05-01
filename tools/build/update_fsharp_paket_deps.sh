#! /usr/bin/env bash

# --- Configuration ---
# Set the path to your F# project directory relative to the Git repository root.
# This directory should contain the paket.dependencies file.
# Example: FSHARP_PROJECT_DIR="apps/fsharp-app-a"
# Example: FSHARP_PROJECT_DIR="libs/fsharp-shared-lib"
FSHARP_PROJECT_DIR="" # <-- ** IMPORTANT: Set this variable to your F# project path **

# --- Script Logic ---

# Find the Git repository root
REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null)

if [ -z "$REPO_ROOT" ]; then
  echo "Error: Not in a Git repository."
  exit 1
fi

echo "Git repository root found at: $REPO_ROOT"

# Determine the script's directory (as provided by the user)
SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

# Full path to the F# project directory
FULL_FSHARP_PROJECT_DIR="$REPO_ROOT/$FSHARP_PROJECT_DIR"

# Check if the F# project directory exists
if [ ! -d "$FULL_FSHARP_PROJECT_DIR" ]; then
  echo "Error: F# project directory not found: $FULL_FSHARP_PROJECT_PROJECT_DIR"
  echo "Please set the FSHARP_PROJECT_DIR variable correctly in the script."
  exit 1
fi

echo "Navigating to F# project directory: $FULL_FSHARP_PROJECT_DIR"

# Navigate to the F# project directory and run the update commands
(
  cd "$FULL_FSHARP_PROJECT_DIR" || { echo "Error: Could not change to directory $FULL_FSHARP_PROJECT_DIR"; exit 1; }

  echo "Restoring dotnet tools and installing Paket dependencies..."
  # Restore dotnet tools (like Paket)
  dotnet tool restore || { echo "Error: dotnet tool restore failed."; exit 1; }

  # Install Paket dependencies
  dotnet paket install || { echo "Error: dotnet paket install failed."; exit 1; }

  echo "Running paket2bazel to update Bazel dependencies..."
  # Run paket2bazel to generate/update Bazel BUILD files
  # We use $(pwd) here which is now the F# project directory
  bazel run @rules_dotnet//tools/paket2bazel -- \
    --dependencies-file "$REPO_ROOT"/paket.dependencies \
    --output-folder "$REPO_ROOT" || { echo "Error: paket2bazel failed."; exit 1; }

  echo "F# dependencies updated successfully."
)

