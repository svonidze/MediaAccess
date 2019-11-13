FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers

# copy and publish app and libraries
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build rintime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Telegram.BotServer.dll"]