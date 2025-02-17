using CareNavigatorSparrow.Core.Interfaces;
using CareNavigatorSparrow.Core.Services;
using CareNavigatorSparrow.Infrastructure.Data;
using CareNavigatorSparrow.Infrastructure.Data.Queries;
using CareNavigatorSparrow.UseCases.Contributors.List;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace CareNavigatorSparrow.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    //string? connectionString = config.GetConnectionString("SqliteConnection");

    //Guard.Against.Null(connectionString);

    //services.AddDbContext<AppDbContext>(options =>
    // options.UseSqlite(connectionString));

    string? connectionString = config.GetConnectionString("carenavigator"); //the PGsql name from aspire
    Guard.Against.Null(connectionString);

    //seems to be that the aspire builder AddNpgsqlDbContext sets the dbcontext up for us
    // NOPE ... see this https://github.com/dotnet/aspire/discussions/6967
    services.AddDbContext<AppDbContext>(options =>
       {
         options
          .UseNpgsql(connectionString)
          .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
       });

    //builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgresdb");

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
           .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
           .AddScoped<IDeleteContributorService, DeleteContributorService>();


    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
