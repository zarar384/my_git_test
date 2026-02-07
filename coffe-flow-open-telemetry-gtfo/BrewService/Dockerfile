# build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# копировать весь репозиторий (нужно для Directory.*.props)
COPY . .

# переход в папку сервиса
WORKDIR /src/BrewService

RUN dotnet restore
RUN dotnet publish -c Release -o /app

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app

COPY --from=build /app .
EXPOSE 8080

ENTRYPOINT ["dotnet", "BrewService.dll"]