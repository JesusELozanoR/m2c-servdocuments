FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
RUN apt-get update && apt-get install -y curl unzip

COPY NuGet.Config ./
COPY ServDocumentos.API.sln ./
COPY . .

# Descargar paquetes NuGet necesarios utilizando curl y autenticación NTLM
RUN curl --ntlm -u 'TKS\pharevalo:Wixi671_Wg%J' -o nuget-packages.zip http://192.168.101.28:8050/Desarrollo/_packaging/DESARROLLO_TEST/nuget/v3/index.json && \
    unzip nuget-packages.zip -d /nuget-packages && rm nuget-packages.zip

# Copiar los paquetes descargados a la carpeta de paquetes de NuGet
RUN mkdir -p ~/.nuget/NuGet/ && cp -r /nuget-packages ~/.nuget/NuGet/

RUN dotnet restore --configfile NuGet.Config --verbosity detailed --ignore-failed-sources
RUN dotnet publish --no-restore ServDocumentos.API/ServDocumentos.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "ServDocumentos.API.dll"]