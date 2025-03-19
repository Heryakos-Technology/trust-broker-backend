# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build-env
WORKDIR /app

# Copy the csproj file and restore dependencies
COPY broker-service.csproj ./
RUN dotnet restore broker-service.csproj

# Copy the project files and build the release
COPY . ./
RUN dotnet publish broker-service.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "broker-service.dll"]