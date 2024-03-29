FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# copy, restore and publish app and libraries
COPY . ./
RUN dotnet publish -c Release -o out -r linux-arm

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0-jammy-arm32v7 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Telegram.BotServer.dll"]