FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY NuGet.Config ./
RUN mkdir -p /root/.nuget/NuGet && cp ./NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN nuget sources update -ValidAuthenticationTypes basic -Name "local_x0020_Tecas_x0020_v3" -Username "TKS\pharevalo" -ClearTextPassword "Wixi671_Wg%J"
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "ServDocumentos.API.dll"]