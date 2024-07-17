# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Instalar herramientas necesarias
RUN apt-get update && apt-get install -y curl unzip file

COPY NuGet.Config ./
COPY ServDocumentos.API.sln ./
COPY . .

# Descargar y verificar el contenido del archivo
RUN curl --ntlm -u 'TKS\pharevalo:Wixi671_Wg%J' -o nuget-packages.zip http://192.168.101.28:8050/Desarrollo/_packaging/DESARROLLO_TEST/nuget/v3/index.json
RUN ls
RUN file nuget-packages.zip
RUN cat nuget-packages.zip

# Si es un archivo JSON en lugar de un zip, maneja la descarga de paquetes de manera diferente
RUN unzip nuget-packages.zip -d /nuget-packages || echo "No se pudo descomprimir, archivo no es un zip válido"

# Configurar NuGet para usar los paquetes descargados, si es aplicable
RUN mkdir -p ~/.nuget/NuGet/ && cp -r /nuget-packages ~/.nuget/NuGet/ || echo "No se encontraron paquetes para copiar"
RUN cat NuGet.Config
# Agregar credenciales explícitas para el feed de NuGet
RUN dotnet nuget add source NuGet.Config --username 'pharevalo' --password 'Wixi671_Wg%J' --store-password-in-clear-text --name 'local Tecas v3' http://192.168.101.28:8050/Desarrollo/_packaging/DESARROLLO_TEST/nuget/v3/index.json
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
