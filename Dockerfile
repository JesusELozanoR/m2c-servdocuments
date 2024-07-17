# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.Config ./
COPY ServDocumentos.API.sln ./
COPY . .

# Agregar credenciales explícitas para el feed de NuGet
RUN dotnet nuget add source --username 'TKS/pharevalo' --password 'Wixi671_Wg%J' --store-password-in-clear-text --name 'local Tecas v3' http://192.168.101.28:8050/Desarrollo/_packaging/DESARROLLO_TEST/nuget/v3/index.json --configfile NuGet.Config
RUN dotnet nuget add source --name 'local Tecas' http://10.10.200.2/TcsNugetServer/nuget --configfile NuGet.Config

RUN dotnet nuget list source

RUN cat NuGet.Config
# Restaurar y publicar el proyecto
RUN dotnet restore --configfile NuGet.Config --verbosity detailed --ignore-failed-sources
RUN dotnet publish --no-restore ServDocumentos.API/ServDocumentos.API.csproj -c Release -o /app

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "ServDocumentos.API.dll"]
