using Customer.API.Middlewares;
using Customer.Application.UseCases;
using Customer.Domain.Ports;
using Customer.Infrastructure.Adapters;
using Customer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging com Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/customer-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Customer Service Microservice",
        Version = "v1.0.0",
        Description = "Microserviço de Gerenciamento de Clientes com Arquitetura Hexagonal",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Banking Team"
        }
    });
    
    // Incluir comentários XML
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "Customer.API.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }
});

// Configurar banco de dados (SQLite por padrão)
var databaseType = Environment.GetEnvironmentVariable("DATABASE_TYPE") ?? "sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CustomerContext>(options =>
{
    if (databaseType.ToLower() == "sqlite")
    {
        var sqliteConnection = connectionString ?? "Data Source=customer.db";
        options.UseSqlite(sqliteConnection);
        Log.Information($"Usando SQLite: {sqliteConnection}");
    }
    else
    {
        var sqlServerConnection = connectionString ?? "Server=(localdb)\\mssqllocaldb;Database=CustomerService;Integrated Security=true;";
        options.UseSqlServer(sqlServerConnection, sqlOptions =>
            sqlOptions.MigrationsAssembly("Customer.Infrastructure")
        );
        Log.Information($"Usando SQL Server: {sqlServerConnection}");
    }
});

// Registrar portas e adaptadores (Injeção de Dependência)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Registrar casos de uso
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<GetCustomerByIdUseCase>();
builder.Services.AddScoped<ListAllCustomersUseCase>();
builder.Services.AddScoped<UpdateCustomerUseCase>();
builder.Services.AddScoped<DeleteCustomerUseCase>();

var app = builder.Build();

// Configurar pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Service API v1.0");
    options.RoutePrefix = "swagger";
});

// Usar middleware de seed
app.UseSeedDatabase();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Log.Information("Iniciando aplicação Customer Service Microservice");
app.Run();
