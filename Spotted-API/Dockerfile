#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Spotter-API/Spotter-API.csproj", "Spotter-API/"]
RUN dotnet restore "Spotter-API/Spotter-API.csproj"
COPY . .
WORKDIR "/src/Spotter-API"
RUN dotnet build "Spotter-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spotter-API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spotter-API.dll"]