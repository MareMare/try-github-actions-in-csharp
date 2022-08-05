# How to build image:
# docker build --pull --rm -f "src/Dockerfile" -t trygithubactionsincsharp:latest "src"
# 
# How to run image:
# docker run --rm -it trygithubactionsincsharp:latest
# 
# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
WORKDIR /app
COPY . ./
RUN dotnet publish ./GithubActions.Csharp/GithubActions.Csharp.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="MareMare"
LABEL repository="https://github.com/MareMare/try-github-actions-in-csharp"
LABEL homepage="https://github.com/MareMare/try-github-actions-in-csharp"

# Label as GitHub action
LABEL com.github.actions.name="try-github-actions-in-csharp"
# Limit to 160 characters
LABEL com.github.actions.description="An example of creating and executing custom GitHub Actions in C#."
# See branding:
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="activity"
LABEL com.github.actions.color="orange"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:6.0
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "/GithubActions.Csharp.dll" ]
