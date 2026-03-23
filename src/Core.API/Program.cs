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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=BankingCore;Trusted_Connection=true;";

builder.Services.AddDbContext<BankingContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.MigrationsAssembly("Core.Infrastructure")
    )
);

// Registrar portas e adaptadores (Injeção de Dependência)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();

// Registrar casos de uso
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<CreateBankAccountUseCase>();
builder.Services.AddScoped<TransferUseCase>();

var app = builder.Build();

// Configurar pipeline HTTP
// Sempre habilitar Swagger para facilitar testes
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Core API v1.0");
    options.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
{
    // Criar banco de dados e aplicar migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BankingContext>();
        dbContext.Database.EnsureCreated();
        Log.Information("Banco de dados criado/verificado");
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Log.Information("Iniciando aplicação Banking Core Microservice");
app.Run();
