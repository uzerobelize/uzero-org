# Monorepo

This is a monorepo managed with Bazel, containing F#, TypeScript, and Python projects.

## Structure

- `apps/`: Contains runnable applications and services.
- `libs/`: Contains reusable libraries and modules.
- `tools/`: Contains shared build tools and custom Bazel rules.
  - `bazel_rules/`: Custom Bazel rule implementations (`.bzl` files).
- `third_party/`: Manages external dependencies.
- `WORKSPACE`: Defines the Bazel workspace and external dependencies.
- `BUILD.bazel`: Root-level build file.
- `.bazelrc`: Bazel configuration.

## Getting Started

1. Ensure Bazel is installed.
2. Clone this repository.
3. Run Bazel commands from the root directory.

## Building

Use `bazel build //...` to build all targets or specify individual targets.

## Testing

Use `bazel test //...` to run all tests or specify individual test targets.
# uzero-org
# uzero-org
# uzero-org
