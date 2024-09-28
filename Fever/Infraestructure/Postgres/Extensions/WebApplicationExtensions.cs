using Microsoft.EntityFrameworkCore;

namespace Fever.Infraestructure.Postgres.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ApplyMigrations<TContext>(this WebApplication app)
        where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        db.Database.Migrate();
        return app;
    }
}
