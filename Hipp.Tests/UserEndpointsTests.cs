using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using Hipp.Application.DTOs.Auth;
using Hipp.Application.DTOs.Users;

namespace Hipp.Tests;

public class UserEndpointsTests
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private string _adminToken;
    private string _testUserId;

    public UserEndpointsTests(ITestOutputHelper output)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
        _output = output;
    }

    private async Task LoginAsAdmin()
    {
        var loginDto = new LoginDto
        {
            Email = "admin@example.com",
            Password = "Admin123!"
        };

        var response = await _client.PostAsync("/api/Auth/login", 
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));
        
        _output.WriteLine($"Login Response: {await response.Content.ReadAsStringAsync()}");
        
        Assert.True(response.IsSuccessStatusCode, $"Login failed: {await response.Content.ReadAsStringAsync()}");
        
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(await response.Content.ReadAsStringAsync());
        _adminToken = result.GetProperty("token").GetString();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
    }

    [Fact]
    public async Task TestFullUserLifecycle()
    {
        // 1. Login as admin
        await LoginAsAdmin();
        Assert.NotNull(_adminToken);

        // 2. Create a new user
        var createUserDto = new CreateUserDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "Test123!",
            PhoneNumber = "1234567890",
            Role = "Menaxher"
        };

        var createResponse = await _client.PostAsync("/api/Users", 
            new StringContent(JsonSerializer.Serialize(createUserDto), Encoding.UTF8, "application/json"));
        
        _output.WriteLine($"Create User Response: {await createResponse.Content.ReadAsStringAsync()}");
        Assert.True(createResponse.IsSuccessStatusCode, $"Create user failed: {await createResponse.Content.ReadAsStringAsync()}");

        var createResult = await JsonSerializer.DeserializeAsync<JsonElement>(await createResponse.Content.ReadAsStringAsync());
        _testUserId = createResult.GetProperty("id").GetString();

        // 3. Get user by ID
        var getUserResponse = await _client.GetAsync($"/api/Users/{_testUserId}");
        Assert.True(getUserResponse.IsSuccessStatusCode);
        var user = await JsonSerializer.DeserializeAsync<JsonElement>(await getUserResponse.Content.ReadAsStringAsync());
        Assert.Equal(createUserDto.Email, user.GetProperty("email").GetString());

        // 4. Get user by email
        var getByEmailResponse = await _client.GetAsync($"/api/Users/email/{createUserDto.Email}");
        Assert.True(getByEmailResponse.IsSuccessStatusCode);

        // 5. Update user profile
        var updateProfileDto = new UpdateProfileDto
        {
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "9876543210"
        };

        var updateResponse = await _client.PutAsync($"/api/Users/{_testUserId}/profile",
            new StringContent(JsonSerializer.Serialize(updateProfileDto), Encoding.UTF8, "application/json"));
        Assert.True(updateResponse.IsSuccessStatusCode);

        // 6. Change password
        var changePasswordDto = new ChangePasswordDto
        {
            UserId = _testUserId,
            NewPassword = "NewTest123!"
        };

        var changePasswordResponse = await _client.PutAsync("/api/Users/change-password",
            new StringContent(JsonSerializer.Serialize(changePasswordDto), Encoding.UTF8, "application/json"));
        Assert.True(changePasswordResponse.IsSuccessStatusCode);

        // 7. Search users
        var searchResponse = await _client.GetAsync($"/api/Users/search?searchTerm={createUserDto.Email}");
        Assert.True(searchResponse.IsSuccessStatusCode);
        var searchResults = await JsonSerializer.DeserializeAsync<JsonElement>(await searchResponse.Content.ReadAsStringAsync());
        Assert.True(searchResults.EnumerateArray().Any());

        // 8. Get user roles
        var getRolesResponse = await _client.GetAsync($"/api/Users/{_testUserId}/roles");
        Assert.True(getRolesResponse.IsSuccessStatusCode);
        var roles = await JsonSerializer.DeserializeAsync<JsonElement>(await getRolesResponse.Content.ReadAsStringAsync());
        Assert.Contains(createUserDto.Role, roles.EnumerateArray().Select(r => r.GetString()));

        // 9. Soft delete user
        var softDeleteResponse = await _client.DeleteAsync($"/api/Users/{_testUserId}/soft");
        Assert.True(softDeleteResponse.IsSuccessStatusCode);

        // 10. Verify user is soft deleted
        var getDeletedUserResponse = await _client.GetAsync($"/api/Users/{_testUserId}");
        Assert.True(getDeletedUserResponse.IsSuccessStatusCode);
        var deletedUser = await JsonSerializer.DeserializeAsync<JsonElement>(await getDeletedUserResponse.Content.ReadAsStringAsync());
        Assert.False(deletedUser.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task TestPasswordResetFlow()
    {
        await LoginAsAdmin();

        // 1. Initiate password reset
        var email = "testuser@example.com";
        var initiateResponse = await _client.PostAsync("/api/Users/initiate-password-reset",
            new StringContent(JsonSerializer.Serialize(email), Encoding.UTF8, "application/json"));
        Assert.True(initiateResponse.IsSuccessStatusCode);

        // Note: In a real test, you'd need to get the reset token from the email
        // For this test, we'll just verify the endpoint responds correctly
        _output.WriteLine("Password reset initiation successful");
    }

    [Fact]
    public async Task TestGetAllUsers()
    {
        await LoginAsAdmin();

        var response = await _client.GetAsync("/api/Users");
        Assert.True(response.IsSuccessStatusCode);

        var users = await JsonSerializer.DeserializeAsync<JsonElement>(await response.Content.ReadAsStringAsync());
        Assert.True(users.EnumerateArray().Any());
    }

    [Fact]
    public async Task TestUserSearch()
    {
        await LoginAsAdmin();

        // Test various search combinations
        var searches = new[]
        {
            "/api/Users/search?pageNumber=1&pageSize=10",
            "/api/Users/search?role=Admin",
            "/api/Users/search?isDeleted=false",
            "/api/Users/search?searchTerm=admin"
        };

        foreach (var searchUrl in searches)
        {
            var response = await _client.GetAsync(searchUrl);
            Assert.True(response.IsSuccessStatusCode, $"Search failed for URL: {searchUrl}");
            var results = await JsonSerializer.DeserializeAsync<JsonElement>(await response.Content.ReadAsStringAsync());
            _output.WriteLine($"Search results for {searchUrl}: {results.EnumerateArray().Count()} users found");
        }
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

        var response = await _client.PostAsync("/api/Users",
            new StringContent(JsonSerializer.Serialize(createUserDto), Encoding.UTF8, "application/json"));
        
        Assert.True(response.IsSuccessStatusCode);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(await response.Content.ReadAsStringAsync());
        return result.GetProperty("id").GetString();
    }
} 