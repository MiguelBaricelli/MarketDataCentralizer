# ================= BUILD STAGE =================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia solution e csproj
COPY MarketDataCentralizer.sln ./

COPY MarketDataCentralizer/MarketDataCentralizer.csproj MarketDataCentralizer/
COPY MarketDataCentralizer.Application/MarketDataCentralizer.Application.csproj MarketDataCentralizer.Application/
COPY MarketDataCentralizer.Domain/MarketDataCentralizer.Domain.csproj MarketDataCentralizer.Domain/
COPY MarketDataCentralizer.Infrastructure/MarketDataCentralizer.Infrastructure.csproj MarketDataCentralizer.Infrastructure/

# Restore
RUN dotnet restore MarketDataCentralizer.sln

# Copia restante do código
COPY . .

# Publish
RUN dotnet publish MarketDataCentralizer/MarketDataCentralizer.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ================= RUNTIME STAGE =================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Usuário não-root
RUN adduser --disabled-password --gecos "" appuser

# Copia aplicação
COPY --from=build /app/publish .

# Permissões
RUN chown -R appuser /app

USER appuser

# Porta Render
EXPOSE 10000

# ASP.NET
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MarketDataCentralizer.dll"]