using Customer.Infrastructure.Persistence;

namespace Customer.API.Middlewares;

/// <summary>
/// Middleware para inicializar o banco de dados com seed
/// </summary>
public class SeedDatabaseMiddleware
{
    private readonly RequestDelegate _next;
    private static bool _seeded = false;

    public SeedDatabaseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CustomerContext dbContext)
    {
        if (!_seeded)
        {
            // Criar banco de dados e schema
            await dbContext.Database.EnsureCreatedAsync();
            
            // Seed com dados iniciais
            await CustomerContextSeed.SeedAsync(dbContext);
            _seeded = true;
        }

        await _next(context);
    }
}

public static class SeedDatabaseMiddlewareExtensions
{
    public static IApplicationBuilder UseSeedDatabase(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SeedDatabaseMiddleware>();
    }
}
