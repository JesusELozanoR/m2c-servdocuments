FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY NuGet.Config ./
COPY . .
RUN echo "machine 192.168.101.28 login pharevalo password Wixi671_Wg%J" >> ~/.netrc
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
#COPY ServDocumentos.API/bin/Debug/netcoreapp3.1/* ./
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "ServDocumentos.API.dll"]