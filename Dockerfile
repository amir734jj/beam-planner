FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env

# Turns on the optimization
ARG BUILD_CONFIGURATION=Release

# Install OpenJDK-11
RUN apk --no-cache add openjdk11

WORKDIR /app

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

WORKDIR /app/out

ENTRYPOINT ["dotnet", "/app/out/App.dll"]