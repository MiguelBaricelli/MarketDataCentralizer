FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:10000
ENV DOTNET_GCConserveMemory=9
ENV DOTNET_GCHeapHardLimit=400000000
EXPOSE 10000
ENTRYPOINT ["sh", "-c", "dotnet MarketDataCentralizer.dll 2>&1"]