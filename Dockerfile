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
RUN echo ">>> [1/4] Iniciando restore de dependencias..." && \
    dotnet restore MarketDataCentralizer/MarketDataCentralizer.csproj && \
    echo ">>> [1/4] Restore concluido com sucesso." || \
    { echo ">>> [ERRO] Falha no restore. Abortando."; exit 1; }

# Copia restante do código
COPY . .
RUN echo ">>> [2/4] Codigo fonte copiado."

# Build
RUN echo ">>> [3/4] Iniciando build..." && \
    dotnet build MarketDataCentralizer/MarketDataCentralizer.csproj \
        -c Release \
        --no-restore && \
    echo ">>> [3/4] Build concluido com sucesso." || \
    { echo ">>> [ERRO] Falha no build. Abortando."; exit 1; }

# Publish
RUN echo ">>> [4/4] Iniciando publish..." && \
    dotnet publish MarketDataCentralizer/MarketDataCentralizer.csproj \
        -c Release \
        -o /app/publish \
        --no-restore && \
    echo ">>> [4/4] Publish concluido com sucesso." || \
    { echo ">>> [ERRO] Falha no publish. Abortando."; exit 1; }

# Lista o conteúdo publicado para confirmar
RUN echo ">>> Conteudo do /app/publish:" && \
    ls -la /app/publish/ && \
    echo ">>> DLLs encontradas:" && \
    ls /app/publish/*.dll


# ================= RUNTIME STAGE =================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Usuário não-root
RUN adduser --disabled-password --gecos "" appuser

# Copia aplicação
COPY --from=build /app/publish .

# Confirma o que foi copiado no runtime
RUN echo ">>> Conteudo copiado para runtime /app:" && \
    ls -la /app/ && \
    echo ">>> Verificando DLL principal..." && \
    test -f /app/MarketDataCentralizer.dll && \
    echo ">>> MarketDataCentralizer.dll encontrada. OK." || \
    { echo ">>> [ERRO] MarketDataCentralizer.dll NAO encontrada!"; ls /app/*.dll; exit 1; }

# Permissões
RUN chown -R appuser /app
USER appuser

# Porta Render
EXPOSE 10000

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MarketDataCentralizer.dll"]