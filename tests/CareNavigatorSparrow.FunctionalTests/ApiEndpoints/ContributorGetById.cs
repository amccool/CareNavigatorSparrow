﻿//using CareNavigatorSparrow.Infrastructure.Data;
//using CareNavigatorSparrow.Web.Contributors;


//namespace CareNavigatorSparrow.FunctionalTests.ApiEndpoints;

//[Collection("Sequential")]
//public class ContributorGetById(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
//{
//  private readonly HttpClient _client = factory.CreateClient();

//  [Fact]
//  public async Task ReturnsSeedContributorGivenId1()
//  {
//    var result = await _client.GetAndDeserializeAsync<ContributorRecord>(GetContributorByIdRequest.BuildRoute(1));

//    Assert.Equal(1, result.Id);
//    Assert.Equal(SeedData.Contributor1.Name, result.Name);
//  }

//  [Fact]
//  public async Task ReturnsNotFoundGivenId1000()
//  {
//    string route = GetContributorByIdRequest.BuildRoute(1000);
//    _ = await _client.GetAndEnsureNotFoundAsync(route);
//  }
//}
