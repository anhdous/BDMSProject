# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
WORKDIR /app/BDMSApp
RUN dotnet publish BDMSApp.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/BDMSApp/out .

EXPOSE 10000
ENTRYPOINT ["dotnet", "BDMSApp.dll"]