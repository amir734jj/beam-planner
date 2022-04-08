FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env

# Turns on the optimization
ARG BUILD_CONFIGURATION=Release

# Install OpenJDK-11
RUN apk --no-cache add openjdk11

WORKDIR /app

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

# No need to have java anymore, this will enable caching
FROM mcr.microsoft.com/dotnet/aspnet:5.0-bullseye-slim AS run-env

COPY --from=build-env /app/out /app/out

WORKDIR /app/out

ENTRYPOINT ["dotnet", "/app/out/App.dll"]