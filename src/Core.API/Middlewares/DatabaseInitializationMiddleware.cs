using Core.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;

namespace Core.API.Middlewares;

/// <summary>
/// Middleware que garante que o banco de dados está inicializado com dados de teste
/// </summary>
public class DatabaseInitializationMiddleware
{
    private readonly RequestDelegate _next;
    private static bool _initialized = false;
    private static readonly object _lock = new object();

    public DatabaseInitializationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, BankingContext dbContext, ILogger<DatabaseInitializationMiddleware> logger)
    {
        if (!_initialized)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    try
                    {
                        logger.LogInformation("Inicializando banco de dados...");
                        
                        // Criar banco de dados se não existir
                        dbContext.Database.EnsureCreated();
                        
                        // Executar seed de dados
                        BankingContextSeed.SeedAsync(dbContext).Wait();
                        
                        logger.LogInformation("Banco de dados inicializado com sucesso");
                        _initialized = true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erro ao inicializar banco de dados");
                    }
                }
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extensão para registrar o middleware
/// </summary>
public static class DatabaseInitializationMiddlewareExtensions
{
    public static IApplicationBuilder UseDatabaseInitialization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DatabaseInitializationMiddleware>();
    }
}
