using CareNavigatorSparrow.Core.ContributorAggregate;

namespace CareNavigatorSparrow.Infrastructure.Data;

public static class SeedData
{
  public static readonly Contributor Contributor1 = new("Ardalis");
  public static readonly Contributor Contributor2 = new("Snowfrog");

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded

    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {

    Contributor contrib = new("PG not happy");
    contrib.Id = 1111;
    Contributor contribMore = new("PG not happy, still");
    contrib.Id = 222;


    //dbContext.Contributors.AddRange([Contributor1, Contributor2]);

    dbContext.Contributors.AddRange([contrib, contribMore]);
    await dbContext.SaveChangesAsync();
  }
}
