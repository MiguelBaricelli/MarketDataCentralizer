FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish MarketDataCentralizer/MarketDataCentralizer.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_GCConserveMemory=9
ENV DOTNET_GCHeapHardLimit=400000000

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet MarketDataCentralizer.dll 2>&1"]