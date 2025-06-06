using System.Reflection;
using IdentityServer;
using IS.Entities;
using IS.InitialSeed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IS
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            // uncomment if you want to add a UI
            //builder.Services.AddRazorPages();
            var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("identitySqlConnection")
                )
            );
            builder
                .Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();
            builder
                .Services.AddIdentityServer(options =>
                {
                    // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                    options.EmitStaticAudienceClaim = true;
                })
                .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c =>
                        c.UseSqlServer(
                            builder.Configuration.GetConnectionString("sqlConnection"),
                            sql => sql.MigrationsAssembly(migrationAssembly)
                        );
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o =>
                        o.UseSqlServer(
                            builder.Configuration.GetConnectionString("sqlConnection"),
                            sql => sql.MigrationsAssembly(migrationAssembly)
                        );
                })
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddRazorPages();
            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add a UI
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment if you want to add a UI
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();

            app.MigrateDatabase();
            return app;
        }
    }
}
