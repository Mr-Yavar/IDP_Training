using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IS;
using Microsoft.EntityFrameworkCore;

namespace IS.InitialSeed;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            using (var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>())
            {
                try
                {
                    context.Database.Migrate();
                    if (!context.Clients.Any())
                    {
                        foreach (var client in Config.Clients)
                        {
                            context.Clients.Add(client.ToEntity());
                        }
                        context.SaveChanges();
                    }
                    if (!context.IdentityResources.Any())
                    {
                        foreach (var resource in Config.IdentityResources)
                        {
                            context.IdentityResources.Add(resource.ToEntity());
                        }
                        context.SaveChanges();
                    }
                    if (!context.ApiScopes.Any())
                    {
                        foreach (var apiScope in Config.ApiScopes)
                        {
                            context.ApiScopes.Add(apiScope.ToEntity());
                        }
                        context.SaveChanges();
                    }
                    if (!context.ApiResources.Any())
                    {
                        foreach (var resource in Config.Apis)
                        {
                            context.ApiResources.Add(resource.ToEntity());
                        }
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //Log errors or do anything you think it's needed
                    throw;
                }
            }
        }
        return host;
    }
}
