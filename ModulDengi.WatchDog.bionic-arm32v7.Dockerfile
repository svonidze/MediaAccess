FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy, restore and publish app and libraries
COPY . ./
RUN dotnet publish -c Release -o out -r linux-arm

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-bionic-arm32v7 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "ModulDengi.WatchDog.dll"]