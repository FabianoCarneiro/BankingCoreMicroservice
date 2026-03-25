using Core.API.Middlewares;
using Core.Application.UseCases;
using Core.Domain.Ports;
using Core.Infrastructure.Adapters;
using Core.Infrastructure.Notifications;
using Core.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging com Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/banking-core-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Banking Core Microservice",
        Version = "v1.0.0",
        Description = "Microserviço de Core Bancário com Arquitetura Hexagonal",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Banking Team"
        }
    });
    
    // Incluir comentários XML
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "Core.API.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }
});

// Configurar banco de dados
// Suporte para SQLite (padrão) ou SQL Server
var databaseType = Environment.GetEnvironmentVariable("DATABASE_TYPE") ?? "sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BankingContext>(options =>
{
    if (databaseType.ToLower() == "sqlite")
    {
        // SQLite - simples, sem instalação necessária
        var sqliteConnection = connectionString ?? "Data Source=banking.db";
        options.UseSqlite(sqliteConnection);
        Log.Information($"Usando SQLite: {sqliteConnection}");
    }
    else
    {
        // SQL Server - padrão original
        var sqlServerConnection = connectionString ?? "Server=(localdb)\\mssqllocaldb;Database=BankingCore;Trusted_Connection=true;";
        options.UseSqlServer(sqlServerConnection, sqlOptions =>
            sqlOptions.MigrationsAssembly("Core.Infrastructure")
        );
        Log.Information($"Usando SQL Server: {sqlServerConnection}");
    }
});

// Registrar portas e adaptadores (Injeção de Dependência)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();

// Registrar casos de uso de clientes
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<GetCustomerByIdUseCase>();
builder.Services.AddScoped<ListAllCustomersUseCase>();

// Registrar casos de uso
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<CreateBankAccountUseCase>();
builder.Services.AddScoped<TransferUseCase>();

var app = builder.Build();

// Configurar pipeline HTTP
// Middleware para inicializar banco de dados com dados de teste
app.UseDatabaseInitialization();

// Sempre habilitar Swagger para facilitar testes
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Core API v1.0");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Log.Information("Iniciando aplicação Banking Core Microservice");
app.Run();
