# try-github-actions-in-csharp

[![An example of creating and executing custom GitHub Actions in C#.](https://github.com/MareMare/try-github-actions-in-csharp/actions/workflows/main.yml/badge.svg)](https://github.com/MareMare/try-github-actions-in-csharp/actions/workflows/main.yml)

An example of creating and executing custom GitHub Actions in C#.

C#でカスタムな GitHub Actions の作成と実行例です。

~~今のところ失敗~~

以下が間違ってた。
* `Dockerfile` ファイル名
  * ❌ `DockerFile`：`F`は`f`
  * 🆗 `Dockerfile`
* `ENTRYPOINT`
  * ❌
    ``` Dockerfile
     # Relayer the .NET SDK, anew with the build output
    FROM mcr.microsoft.com/dotnet/sdk:6.0
    WORKDIR /app                                        # ❌ 不要！
    COPY --from=build-env /app/out .
    ENTRYPOINT [ "dotnet", "GithubActions.Csharp.dll" ] # ❌ `"GithubActions.Csharp.dll"` に `/` が必要！
    ```
  * 🆗
    ``` Dockerfile
     # Relayer the .NET SDK, anew with the build output
    FROM mcr.microsoft.com/dotnet/sdk:6.0
    COPY --from=build-env /app/out .
    ENTRYPOINT [ "dotnet", "/GithubActions.Csharp.dll" ]
    ```
