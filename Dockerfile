# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ProductApi.Solution.sln .
COPY src/ProductApi.Domain/ProductApi.Domain.csproj src/ProductApi.Domain/
COPY src/ProductApi.Application/ProductApi.Application.csproj src/ProductApi.Application/
COPY src/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj src/ProductApi.Infrastructure/
COPY src/ProductApi.API/ProductApi.API.csproj src/ProductApi.API/
COPY tests/ProductApi.Tests/ProductApi.Tests.csproj tests/ProductApi.Tests/

RUN dotnet restore ProductApi.Solution.sln

COPY . .
RUN dotnet publish src/ProductApi.API/ProductApi.API.csproj -c Release -o /app/publish --no-restore

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductApi.API.dll"]
