# try-github-actions-in-csharp

⚠️ **_This repository is intended for me._**

[![An example of creating and executing custom GitHub Actions in C#.](https://github.com/MareMare/try-github-actions-in-csharp/actions/workflows/main.yml/badge.svg)](https://github.com/MareMare/try-github-actions-in-csharp/actions/workflows/main.yml)

An example of creating and executing custom GitHub Actions in C#.

C#でカスタムな GitHub Actions の作成と実行例です。

以下、個人的な注意点。
* `Dockerfile` ファイル名: `F` ---> `f`
  * ❌ `DockerFile`
  * 🆗 `Dockerfile`
* `ENTRYPOINT`
  * ❌
    ``` Dockerfile
     # Relayer the .NET SDK, anew with the build output
    FROM mcr.microsoft.com/dotnet/sdk:6.0
    WORKDIR /app                                        # (A) 
    COPY --from=build-env /app/out .
    ENTRYPOINT [ "dotnet", "GithubActions.Csharp.dll" ] # ❌ (A) と不一致: `/` が必要
    ```
  * 🆗
    ``` Dockerfile
     # Relayer the .NET SDK, anew with the build output
    FROM mcr.microsoft.com/dotnet/sdk:6.0
    WORKDIR /app                                        # (A) 
    COPY --from=build-env /app/out .
    ENTRYPOINT [ "dotnet", "/app/GithubActions.Csharp.dll" ]
    ```

## src/Dockerfile
```Dockerfile
# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
WORKDIR /src
COPY . ./
RUN dotnet publish ./GithubActions.Csharp/GithubActions.Csharp.csproj -c Release -o out --no-self-contained

# Set the final image as the .NET 6.0 runtime, anew with the build output.
FROM mcr.microsoft.com/dotnet/runtime:6.0 as final
WORKDIR /app
COPY --from=build-env /src/out .
ENTRYPOINT [ "dotnet", "/app/GithubActions.Csharp.dll" ]
```

## src/action.yml
```yml
name: 'try-github-actions-in-csharp'
description: 'An example of creating and executing custom GitHub Actions in C#.'
branding:
  icon: sliders
  color: purple
inputs:
  firstName:
    description: 'First Name.'
    required: true
  lastName:
    description: 'Last Name.'
    required: true
outputs:
  summary-details:
    description: 'A detailed summary of all the projects that were flagged.'
runs:
  using: 'docker'
  image: 'Dockerfile'
  args:
    - '--first-name'
    - ${{ inputs.firstName }}
    - '--last-name'
    - ${{ inputs.lastName }}
```

## .github/workflows/main.yml
```yml
# The name of the work flow. Badges will use this name
name: 'An example of creating and executing custom GitHub Actions in C#.'

on:
  push:
    branches: [ main ]
  workflow_dispatch:
    inputs:
      reason:
        description: 'The reason for running the workflow'
        required: true
        default: 'Manual run'

jobs:
  try-github-actions-in-csharp:

    runs-on: ubuntu-latest
    permissions:
      contents: write
      pull-requests: write

    steps:
    - uses: actions/checkout@v3

    - name: 'Print manual run reason'
      if: ${{ github.event_name == 'workflow_dispatch' }}
      run: |
        echo 'Reason: ${{ github.event.inputs.reason }}'

    - name: GithubActions.Csharp
      id: github-actions-csharp
      uses: ./src # Uses an action in this directory
      env:
        # Pass the environment variables to the C# module on the Docker container.
        GH_TOKEN: ${{ secrets.GITHUB_TOKEN }} 
      with:
        firstName: ${{ github.repository_owner }}
        lastName: ${{ github.repository }}
      
    # Use the output from the `hello` step
    - name: Get the output
      run: echo "The output was ${{ steps.github-actions-csharp.outputs.summary-details }}"
```

## Actions の実行結果
Actions を実行した結果を以下に追記してみます。

* updated! 2022/08/13 09:48:24(UTC)
