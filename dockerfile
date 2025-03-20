# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build-env
RUN apt-get update \
    && apt-get install -y --no-install-recommends \ 
    clang zlib1-dev

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY broker-service.csproj ./
RUN dotnet restore broker-service.csproj

# Copy the project files and build the release
COPY . ./
WORKDIR "/src"
RUN dotnet build "./broker-service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./broker-service.csproj" -c Release -o /app/publish /p:UseAppHost=true

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
COPY --from=publish /app/publish .
ENTRYPOINT ["./"]