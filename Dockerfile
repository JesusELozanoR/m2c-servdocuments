FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore --verbosity detailed
RUN dotnet publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY ServDocumentos.API/bin/Debug/netcoreapp3.1/* ./
EXPOSE 1433
EXPOSE 80
ENTRYPOINT ["dotnet", "ServDocumentos.API.dll"]