//using CareNavigatorSparrow.Infrastructure.Data;
//using CareNavigatorSparrow.Web.Contributors;

//namespace CareNavigatorSparrow.FunctionalTests.ApiEndpoints;

//[Collection("Sequential")]
//public class ContributorList(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
//{
//  private readonly HttpClient _client = factory.CreateClient();

//  [Fact]
//  public async Task ReturnsTwoContributors()
//  {
//    var result = await _client.GetAndDeserializeAsync<ContributorListResponse>("/Contributors");

//    Assert.Equal(2, result.Contributors.Count);
//    Assert.Contains(result.Contributors, i => i.Name == SeedData.Contributor1.Name);
//    Assert.Contains(result.Contributors, i => i.Name == SeedData.Contributor2.Name);
//  }
//}
