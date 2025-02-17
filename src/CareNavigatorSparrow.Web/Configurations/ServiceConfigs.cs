using CareNavigatorSparrow.Core.Interfaces;
using CareNavigatorSparrow.Infrastructure;
using CareNavigatorSparrow.Infrastructure.Data;
using CareNavigatorSparrow.Infrastructure.Email;

namespace CareNavigatorSparrow.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {

    // DO NOT use this style
    //builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "carenavigator"); // "postgresdb");


    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatrConfigs();

    //a little weird, David Fowl is struggling here https://github.com/dotnet/aspire/discussions/6967
    builder.EnrichNpgsqlDbContext<AppDbContext>(); //should this get a connection string?    "carenavigator"


    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server
      // See: https://ardalis.com/configuring-a-local-test-email-server/
      //services.AddScoped<IEmailSender, MimeKitEmailSender>();

      // Otherwise use this:
      builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
    }
    else
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    logger.LogInformation("{Project} services registered", "Mediatr and Email Sender");

    return services;
  }


}
