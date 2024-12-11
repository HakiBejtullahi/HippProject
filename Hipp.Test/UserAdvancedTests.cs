using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hipp.Application.DTOs.Auth;
using Hipp.Application.DTOs.Users;

namespace Hipp.Test;

[TestClass]
public class UserAdvancedTests
{
    private readonly HttpClient _client;
    private string? _authToken;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserAdvancedTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5185/");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task AdminLogin()
    {
        var loginDto = new LoginDto
        {
            Email = "admin@example.com",
            Password = "Admin123!"
        };

        var response = await _client.PostAsync("api/auth/login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        Assert.IsTrue(response.IsSuccessStatusCode, $"Admin login failed: {await response.Content.ReadAsStringAsync()}");
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
            await response.Content.ReadAsStringAsync(), 
            _jsonOptions);

        _authToken = loginResponse.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    [TestMethod]
    public async Task UpdateUserProfile_ShouldSucceed()
    {
        // Arrange - Create a test user first
        await AdminLogin();
        var userId = await CreateTestUser();

        var updateProfileDto = new UpdateProfileDto
        {
            UserId = userId,
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "9876543210"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(updateProfileDto, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync("api/users/profile", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Update Profile Response Status: {response.StatusCode}");
        Console.WriteLine($"Update Profile Response Content: {responseContent}");

        // Assert
        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to update profile: {response.StatusCode} - {responseContent}");

        // Verify the update
        var getUserResponse = await _client.GetAsync($"api/users/{userId}");
        var user = JsonSerializer.Deserialize<UserDto>(
            await getUserResponse.Content.ReadAsStringAsync(), 
            _jsonOptions);
        Assert.IsNotNull(user, "User should not be null after update");
        Assert.AreEqual("Updated", user.FirstName);
        Assert.AreEqual("Name", user.LastName);
    }

    [TestMethod]
    public async Task SearchUsers_ShouldReturnResults()
    {
        await AdminLogin();

        // Create a few test users with unique characteristics
        await CreateTestUser("Menaxher");
        await CreateTestUser("Etiketues");
        await CreateTestUser("Komercialist");

        // Test different search scenarios
        var searchScenarios = new[]
        {
            "api/users/search?searchTerm=Test&role=Menaxher",
            "api/users/search?searchTerm=Test&isDeleted=false",
            "api/users/search?searchTerm=Test",
            "api/users/search?searchTerm=Test&pageNumber=1&pageSize=2"
        };

        foreach (var searchUrl in searchScenarios)
        {
            var response = await _client.GetAsync(searchUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Search Response for {searchUrl}");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Content: {responseContent}");

            Assert.IsTrue(response.IsSuccessStatusCode, 
                $"Search failed for {searchUrl}: {response.StatusCode} - {responseContent}");
            
            var results = JsonSerializer.Deserialize<List<UserDto>>(responseContent, _jsonOptions);
            Assert.IsTrue(results?.Any() ?? false, $"No results found for {searchUrl}");
        }
    }

    [TestMethod]
    public async Task GetUserRoles_ShouldReturnCorrectRole()
    {
        await AdminLogin();
        var userId = await CreateTestUser("Menaxher");

        var response = await _client.GetAsync($"api/users/{userId}/role");
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Get Roles Response Status: {response.StatusCode}");
        Console.WriteLine($"Get Roles Response Content: {responseContent}");

        Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to get user role: {response.StatusCode} - {responseContent}");
        var role = JsonSerializer.Deserialize<string>(responseContent, _jsonOptions);
        Assert.IsNotNull(role, "Role should not be null");
        Assert.AreEqual("Menaxher", role);
    }

    private async Task<string> CreateTestUser(string role = "Menaxher")
    {
        var createUserDto = new CreateUserDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "Test123!",
            PhoneNumber = "1234567890",
            Role = role
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createUserDto, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("api/users", content);
        Assert.IsTrue(response.IsSuccessStatusCode, 
            $"Failed to create test user: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

        var createdUser = JsonSerializer.Deserialize<UserDto>(
            await response.Content.ReadAsStringAsync(), 
            _jsonOptions);
        return createdUser.Id;
    }

    private class LoginResponse
    {
        public string? Token { get; set; }
        public UserDto? User { get; set; }
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client.Dispose();
    }
} 