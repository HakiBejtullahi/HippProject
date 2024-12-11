using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hipp.Application.DTOs.Users;
using Hipp.Application.DTOs.Auth;
using System.Collections.Generic;
using System.Linq;

namespace Hipp.Test
{
    [TestClass]
    public class UserEndpointsTests
    {
        private readonly HttpClient _client;
        private string? _authToken;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserEndpointsTests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5185/");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [TestMethod]
        [Priority(1)]
        public async Task ApiServer_ShouldBeRunning()
        {
            try
            {
                // Act
                var response = await _client.GetAsync("api/users");

                // Assert
                Assert.IsNotNull(response, "Response should not be null");
                Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, 
                    "Should get unauthorized since we're not authenticated");
            }
            catch (HttpRequestException ex)
            {
                Assert.Fail($"API server is not running. Please start the API server first. Error: {ex.Message}");
            }
        }

        [TestMethod]
        [Priority(2)]
        public async Task AdminLogin_ShouldSucceed()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "Admin123!"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto, _jsonOptions),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json"));

            // Act
            var response = await _client.PostAsync("api/auth/login", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Login Response Status: {response.StatusCode}");
            Console.WriteLine($"Login Response Content: {responseContent}");

            Assert.IsTrue(response.IsSuccessStatusCode, $"Admin login failed - Status: {response.StatusCode}, Content: {responseContent}");
            
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
            Assert.IsNotNull(loginResponse, "Login response should not be null");
            Assert.IsNotNull(loginResponse.Token, "Token should not be null");
            Assert.IsNotNull(loginResponse.User, "User should not be null");
            Assert.AreEqual("Admin", loginResponse.User.Role, "User should have Admin role");
            
            _authToken = loginResponse.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        [TestMethod]
        [Priority(3)]
        public async Task CreateUser_WithAdminAuth_ShouldSucceed()
        {
            // Arrange - Make sure we're logged in
            await AdminLogin_ShouldSucceed();

            // Generate a unique email
            var uniqueEmail = $"testuser_{Guid.NewGuid()}@example.com";
            
            var createUserDto = new CreateUserDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = uniqueEmail,
                Password = "Test123!",
                PhoneNumber = "1234567890",
                Role = "Menaxher"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createUserDto, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("api/users", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create User Response Status: {response.StatusCode}");
            Console.WriteLine($"Create User Response Content: {responseContent}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to create user: {response.StatusCode} - {responseContent}");
            var createdUser = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions);
            Assert.IsNotNull(createdUser);
            Assert.AreEqual(uniqueEmail, createdUser.Email);
            Assert.AreEqual("Menaxher", createdUser.Role);
        }

        [TestMethod]
        [Priority(4)]
        public async Task GetUsers_WithAdminAuth_ShouldSucceed()
        {
            // Arrange - Make sure we're logged in
            await AdminLogin_ShouldSucceed();

            // Act
            var response = await _client.GetAsync("api/users");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Get Users Response Status: {response.StatusCode}");
            Console.WriteLine($"Get Users Response Content: {responseContent}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Getting users failed: {response.StatusCode} - {responseContent}");
            var users = JsonSerializer.Deserialize<List<UserDto>>(responseContent, _jsonOptions);
            Assert.IsNotNull(users, "Users list should not be null");
            Assert.IsTrue(users.Any(), "Users list should not be empty");
        }

        [TestMethod]
        [Priority(5)]
        public async Task GetUsers_WithoutAuth_ShouldFail()
        {
            // Arrange - Clear any existing auth
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("api/users");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, 
                "Should get unauthorized when not authenticated");
        }

        [TestMethod]
        [Priority(5)]
        public async Task UpdateUserProfile_ShouldSucceed()
        {
            // Arrange - Create a test user first
            await AdminLogin_ShouldSucceed();
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
            var response = await _client.PutAsync($"api/users/{userId}/profile", content);
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
            Assert.AreEqual("Updated", user.FirstName);
            Assert.AreEqual("Name", user.LastName);
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

        [TestMethod]
        [Priority(6)]
        public async Task ChangePassword_ShouldSucceed()
        {
            // Arrange - Create a test user first
            await AdminLogin_ShouldSucceed();
            var userId = await CreateTestUser();

            var changePasswordDto = new ChangePasswordDto
            {
                UserId = userId,
                NewPassword = "NewTest123!"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(changePasswordDto, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync("api/users/change-password", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Change Password Response Status: {response.StatusCode}");
            Console.WriteLine($"Change Password Response Content: {responseContent}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to change password: {response.StatusCode} - {responseContent}");
        }

        public class LoginResponse
        {
            public string? Token { get; set; }
            public UserDto? User { get; set; }
        }
    }
}