using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Application.Services.Authorization;
using MarketDataCentralizer.Application.Services.Daily;
using MarketDataCentralizer.Application.Services.DataMarketBrazil;
using MarketDataCentralizer.Application.Services.Dividends;
using MarketDataCentralizer.Application.Services.General;
using MarketDataCentralizer.Application.Services.Overview;
using MarketDataCentralizer.Application.Services.Redis;
using MarketDataCentralizer.Application.Services.Weekly;
using MarketDataCentralizer.Domain.Models.ApiClientSecurity;
using MarketDataCentralizer.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================= USER SECRETS =================
builder.Configuration.AddUserSecrets<Program>();


/*
var apiKey = builder.Configuration["ApiKeys:KeyApiFinance"];

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Chave da api não encontrada no User Secrets");
    throw new ArgumentException("Chave da api não encontrada no User Secrets");
}
*/


// ================= API KEY (Alpha Vantage) =================
var alphaVantageApiKey = builder.Configuration["ApiKeys:AlphaVantage"];

if (string.IsNullOrEmpty(alphaVantageApiKey))
{
    Console.WriteLine("Chave da API Alpha Vantage não encontrada");
    throw new ArgumentException("Chave da API Alpha Vantage não encontrada");
}

// ================= JWT SETTINGS =================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Auth:Jwt")
);

var jwt = builder.Configuration.GetSection("Auth:Jwt");

if (string.IsNullOrWhiteSpace(jwt["SecretKey"]))
    throw new Exception("JWT SecretKey não configurada");

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

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["SecretKey"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT falhou: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT válido");
                return Task.CompletedTask;
            }
        };

    });


// ================= APPLICATION SERVICES =================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFinanceSummaryVarianceService, FinanceSummaryVarianceService>();
builder.Services.AddScoped<IWeeklyDataForConsultService, WeeklyDataForConsultService>();
builder.Services.AddScoped<IDataOverviewService, DataOverviewService>();
builder.Services.AddScoped<IGeneralResponseService, GeneralResponseService>();
builder.Services.AddScoped<IDailyConsultService, DailyConsultService>();
builder.Services.AddScoped<IDataMarketBrazilService, DataMarketBrazilService>();
builder.Services.AddScoped<RedisTestService>();
builder.Services.AddScoped<ICacheValidator, CacheValidator>();
builder.Services.AddScoped<IStockDividendsService, StockDividendsService>();

// ================= INFRASTRUCTURE =================
builder.Services.AddHttpClient();
builder.Services.AddDependencyInjection(builder.Configuration);

// ================= CONTROLLERS =================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

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
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ================= BUILD =================
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ================= PIPELINE =================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
