using MarketDataCentralizer.Application.DataCollector;
using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Application.Messaging.Orchestrator;
using MarketDataCentralizer.Application.Services.Authorization;
using MarketDataCentralizer.Application.Services.Daily;
using MarketDataCentralizer.Application.Services.DataMarketBrazil;
using MarketDataCentralizer.Application.Services.Dividends;
using MarketDataCentralizer.Application.Services.General;
using MarketDataCentralizer.Application.Services.MarketSituation;
using MarketDataCentralizer.Application.Services.Overview;
using MarketDataCentralizer.Application.Services.Redis;
using MarketDataCentralizer.Application.Services.Weekly;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models.ApiClientSecurity;
using MarketDataCentralizer.Infrastructure.DependencyInjection;
using MarketDataCentralizer.Infrastructure.RabbitMq;
using MarketDataCentralizer.Infrastructure.RabbitMq.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================= LOGGING =================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Logger de bootstrap (antes do DI estar pronto)
using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});
var bootstrapLogger = loggerFactory.CreateLogger("Startup");

bootstrapLogger.LogInformation("Iniciando configuração da aplicação...");

// ================= USER SECRETS =================
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    bootstrapLogger.LogDebug("User Secrets carregados (ambiente de desenvolvimento).");
}

// ================= API KEY (Alpha Vantage) =================
var alphaVantageApiKey = builder.Configuration["ApiKeys:AlphaVantage"];

if (string.IsNullOrEmpty(alphaVantageApiKey))
{
    bootstrapLogger.LogCritical(
        "Chave da API Alpha Vantage não encontrada. " +
        "Verifique se 'ApiKeys:AlphaVantage' está configurado em User Secrets ou variáveis de ambiente.");
    throw new InvalidOperationException("Chave da API Alpha Vantage não configurada.");
}

bootstrapLogger.LogInformation("Chave da API Alpha Vantage carregada com sucesso.");

// ================= JWT SETTINGS =================
var jwtSection = builder.Configuration.GetSection("Auth:Jwt");

if (!jwtSection.Exists())
{
    bootstrapLogger.LogCritical(
        "Seção de configuração 'Auth:Jwt' não encontrada. " +
        "Verifique appsettings.json ou User Secrets.");
    throw new InvalidOperationException("Configuração JWT ausente: seção 'Auth:Jwt' não encontrada.");
}

builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSecretKey = jwtSection["SecretKey"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

var jwtErrors = new List<string>();

if (string.IsNullOrWhiteSpace(jwtSecretKey))
    jwtErrors.Add("'Auth:Jwt:SecretKey' está ausente ou vazia");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    jwtErrors.Add("'Auth:Jwt:Issuer' está ausente ou vazia");

if (string.IsNullOrWhiteSpace(jwtAudience))
    jwtErrors.Add("'Auth:Jwt:Audience' está ausente ou vazia");

if (jwtErrors.Count > 0)
{
    foreach (var error in jwtErrors)
        bootstrapLogger.LogCritical("Configuração JWT inválida: {Error}", error);

    throw new InvalidOperationException(
        $"Configuração JWT incompleta. Problemas encontrados: {string.Join("; ", jwtErrors)}");
}

bootstrapLogger.LogInformation("Configurações JWT validadas com sucesso (Issuer: {Issuer}, Audience: {Audience}).",
    jwtIssuer, jwtAudience);

// ================= AUTHENTICATION =================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Usa o logger injetado pelo pipeline, não o bootstrapLogger
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                logger.LogWarning(
                    context.Exception,
                    "Falha na autenticação JWT. Path: {Path} | Erro: {Message}",
                    context.HttpContext.Request.Path,
                    context.Exception.Message);

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                var user = context.Principal?.Identity?.Name ?? "desconhecido";
                logger.LogDebug("JWT validado com sucesso para o usuário: {User}", user);

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                logger.LogWarning(
                    "Acesso negado (401). Path: {Path} | Motivo: {ErrorDescription}",
                    context.HttpContext.Request.Path,
                    context.ErrorDescription ?? "Token ausente ou inválido");

                return Task.CompletedTask;
            }
        };
    });

bootstrapLogger.LogInformation("Autenticação JWT configurada.");

// ================= APPLICATION SERVICES =================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFinanceSummaryVarianceService, FinanceSummaryVarianceService>();
builder.Services.AddScoped<IWeeklyDataForConsultService, WeeklyDataForConsultService>();
builder.Services.AddScoped<IDataOverviewService, DataOverviewService>();
builder.Services.AddScoped<IGeneralResponseService, GeneralResponseService>();
builder.Services.AddScoped<IDailyConsultService, DailyConsultService>();
builder.Services.AddScoped<IDataMarketBrazilService, DataMarketBrazilService>();
builder.Services.AddScoped<RedisTestService>();
builder.Services.AddScoped<IStockDividendsService, StockDividendsService>();
builder.Services.AddScoped<MarketSituationService>();

builder.Services.AddScoped<MarketDataCollector>();
builder.Services.AddScoped<MarketDataSyncOrchestrator>();

// ================= INFRASTRUCTURE =================
builder.Services.AddHttpClient();
builder.Services.AddDependencyInjection(builder.Configuration, builder.Environment,bootstrapLogger);

builder.Services.AddRabbitMq(builder.Configuration);

//REDIS
builder.Services.AddScoped<ICacheValidator>(sp =>
{
    var cache = sp.GetRequiredService<ICacheRepository>();
    var cacheLogger = sp.GetRequiredService<ILogger<CacheValidator>>();
    return new CacheValidator(cache, cacheLogger, builder.Environment.EnvironmentName);
});
bootstrapLogger.LogInformation("Redis configurado com sucesso. Ambiente: {Environment}", builder.Environment.EnvironmentName);


// ================= CONTROLLERS =================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

// ================= SWAGGER =================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Market Data Centralizer API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe apenas o token JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ================= BUILD =================
var app = builder.Build();
var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
appLogger.LogInformation("Aplicação construída com sucesso. Ambiente: {Environment}",
    app.Environment.EnvironmentName);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ================= PIPELINE =================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    appLogger.LogInformation("Swagger habilitado (ambiente de desenvolvimento).");
}

app.MapControllers();

appLogger.LogInformation("Aplicação iniciada e pronta para receber requisições.");
app.Run();