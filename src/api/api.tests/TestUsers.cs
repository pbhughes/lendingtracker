namespace api.tests;
using System.Net.Http.Json;
using LendingTrackerApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi.Validations;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class TestUsers : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TestUsers(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task AddEndpoint_ShoulReturnUser()
    {
        //act
        var response = await _client.GetAsync("/users");
        var result = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}