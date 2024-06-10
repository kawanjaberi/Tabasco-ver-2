ARG REPO=docker.ofood.cloud/library/microsoft/dotnet

# Use the official .NET Core SDK as a parent image
FROM $REPO/aspnet:8.0.3-alpine3.19-amd64_ofdv1 AS base

WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#ENV ASPNETCORE_HTTP_PORTS=80
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
# RUN apk add --no-cache
# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM $REPO/sdk:8.0.203-alpine3.19-amd64 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build "Tabasco.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tabasco.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tabasco.dll"]


