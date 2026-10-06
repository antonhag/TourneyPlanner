using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourneyPlanner.Infrastructure.Data;

namespace Api.Tests;

public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                           typeof(DbContextOptions<TourneyPlannerDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var configDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                                 typeof(IDbContextOptionsConfiguration<
                                                                     TourneyPlannerDbContext>));
            if (configDescriptor != null)
            {
                services.Remove(configDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                                       typeof(DbContextOptions<
                                                                           TourneyPlannerDbContext>));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddSingleton<SqliteConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });

            // Autentiserar varje testanrop inloggad som testanvändare som API:et använder
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.AddDbContext<TourneyPlannerDbContext>((container, options) =>
                {
                    var connection = container.GetService<SqliteConnection>();
                    options.UseSqlite(connection);
                });
            }
        );
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();
        
        db.Database.EnsureCreated();
        
        return host;
    }
}
