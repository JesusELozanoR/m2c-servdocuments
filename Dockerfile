# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.Config ./
COPY ServDocumentos.API.sln ./
COPY . .

# Verificar la configuración de NuGet
RUN dotnet nuget list source --configfile NuGet.Config
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
