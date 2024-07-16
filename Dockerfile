# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el resto de los archivos del proyecto
COPY . .

# Restaurar dependencias
RUN dotnet restore

# Compilar y publicar el proyecto
RUN dotnet publish -c Release -o /app

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "ServDocumentos.API.dll"]
