using CareNavigatorSparrow.Infrastructure.Data;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;

namespace CareNavigatorSparrow.FunctionalTests;

//public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class, IAsyncLifetime<CustomWebApplicationFactory<TProgram>>
public class CustomWebApplicationFactory : WebApplicationFactory<Program> , IAsyncLifetime
{
  private readonly IResourceBuilder<PostgresDatabaseResource> _db;
  private readonly IHost _app;
  private string? _postgresConnectionString;

  public CustomWebApplicationFactory()
  {
    var options = new DistributedApplicationOptions { AssemblyName = typeof(CustomWebApplicationFactory).Assembly.FullName, DisableDashboard = true };
    var appBuilder = DistributedApplication.CreateBuilder(options);

    _db = appBuilder.AddPostgres("postgres")
      .AddDatabase("carenavigator");

    _app = appBuilder.Build();
  }


  /// <summary>
  /// Overriding CreateHost to avoid creating a separate ServiceProvider per this thread:
  /// https://github.com/dotnet-architecture/eShopOnWeb/issues/465
  /// </summary>
  /// <param name="builder"></param>
  /// <returns></returns>
  protected override IHost CreateHost(IHostBuilder builder)
  {
    builder.UseEnvironment("Development"); // will not send real emails

    builder.ConfigureHostConfiguration(config =>
    {
      config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"ConnectionStrings:{_db.Resource.Name}", _postgresConnectionString },
                });
    });



    var host = builder.Build();
    //host.Start();

    // Get service provider.
    var serviceProvider = host.Services;

    // Create a scope to obtain a reference to the database
    // context (AppDbContext).
    using (var scope = serviceProvider.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<AppDbContext>();

      var logger = scopedServices
          .GetRequiredService<ILogger<CustomWebApplicationFactory>>();

      // Reset Sqlite database for each test run
      // If using a real database, you'll likely want to remove this step.
      db.Database.EnsureDeleted();

      // Ensure the database is created.
      db.Database.EnsureCreated();

      try
      {
        // Can also skip creating the items
        //if (!db.ToDoItems.Any())
        //{
        // Seed the database with test data.
        SeedData.PopulateTestDataAsync(db).Wait();
        //}
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {exceptionMessage}", ex.Message);
      }
    }

    return host;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder
        .ConfigureServices(services =>
        {
          // Configure test dependencies here

          //// Remove the app's ApplicationDbContext registration.
          //var descriptor = services.SingleOrDefault(
          //d => d.ServiceType ==
          //    typeof(DbContextOptions<AppDbContext>));

          //if (descriptor != null)
          //{
          //  services.Remove(descriptor);
          //}

          //// This should be set for each individual test run
          //string inMemoryCollectionName = Guid.NewGuid().ToString();

          //// Add ApplicationDbContext using an in-memory database for testing.
          //services.AddDbContext<AppDbContext>(options =>
          //{
          //  options.UseInMemoryDatabase(inMemoryCollectionName);
          //});
        });
  }

  public new async Task DisposeAsync()
  {
    await base.DisposeAsync();
    await _app.StopAsync();
    if (_app is IAsyncDisposable asyncDisposable)
    {
      await asyncDisposable.DisposeAsync().ConfigureAwait(false);
    }
    else
    {
      _app.Dispose();
    }
  }

  public async Task InitializeAsync()
  {
    await _app.StartAsync();
    _postgresConnectionString = await _db.Resource.ConnectionStringExpression.GetValueAsync(CancellationToken.None);
      //.GetValueAsync()   .    .GetConnectionStringAsync();
  }
}
