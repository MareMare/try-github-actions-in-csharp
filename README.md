# try-github-actions-in-csharp

⚠️ **_This repository is intended for me._**

[![An example of creating and executing custom GitHub Actions in C#.](https://github.com/MareMare/try-github-actions-in-csharp/actions/workflows/main.yml/badge.svg)](https://github.com/MareMare/try-github-actions-in-csharp/actions/workflows/main.yml)

An example of creating and executing custom GitHub Actions in C#.

C#でカスタムな GitHub Actions の作成と実行例です。

~~今のところ失敗~~

以下が間違ってた。
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
