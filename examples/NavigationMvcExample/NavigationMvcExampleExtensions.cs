using System;
using Microsoft.EntityFrameworkCore;

public static class NavigationMvcExampleExtensions
{
    public static IApplicationBuilder MigrateDbContext<T>(this IApplicationBuilder app) where T : DbContext
    {
        using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<T>().Database.Migrate();
        }


        return app;
    }
}