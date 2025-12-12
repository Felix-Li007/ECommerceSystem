namespace UserService.Tests.Integration.API
{
    using System.Net;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc.Testing;
    using UserService.Application.DTOs;
    using UserService.Infrastructure.Data;
    using Xunit;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.AspNetCore.Hosting;

    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _baseUrl = "/api/users";

        public UsersControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<UserDbContext>>();

                    services.AddDbContext<UserDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid());
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                    db.Database.EnsureCreated();
                });
            });

            _client = _factory.CreateClient();
        }

        #region GET Tests

        [Fact]
        public async Task GetAll_WhenNoUsers_ShouldReturnEmptyArray()
        {
            // Act
            var response = await _client.GetAsync(_baseUrl);
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            users.Should().NotBeNull();
            users.Should().BeEmpty();
        }



        [Fact]
        public async Task GetById_WhenUserDoesNotExist_ShouldReturn404()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"{_baseUrl}/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        public async Task GetByEmail_WhenUserDoesNotExist_ShouldReturn404()
        {
            // Act
            var response = await _client.GetAsync($"{_baseUrl}/email/nonexistent@example.com");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task Create_WithoutPhoneNumber_ShouldSucceed()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "usernophone",
                Email = "nophone@example.com",
                Password = "Password@123",
                FullName = "User Without Phone"
                // PhoneNumber not provided
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrl, createDto);
            var user = await response.Content.ReadFromJsonAsync<UserDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            user!.PhoneNumber.Should().BeNullOrEmpty();
        }

        #endregion

        #region PUT Tests


        [Fact]
        public async Task Update_WhenUserDoesNotExist_ShouldReturn404()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FullName = "Updated Name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrl}/{nonExistentId}", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturn400()
        {
            // Arrange
            var updateDto = new UpdateUserDto
            {
                FullName = "Updated Name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrl}/invalid-id", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region DELETE Tests


        [Fact]
        public async Task Delete_WhenUserDoesNotExist_ShouldReturn404()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"{_baseUrl}/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Authentication Tests



        [Fact]
        public async Task Authenticate_WithNonExistentUser_ShouldReturn401()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync($"{_baseUrl}/authenticate", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Change Password Tests





        [Fact]
        public async Task ChangePassword_WhenUserDoesNotExist_ShouldReturn404()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var changeDto = new ChangePasswordDto
            {
                CurrentPassword = "OldPassword@123",
                NewPassword = "NewPassword@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                $"{_baseUrl}/{nonExistentId}/change-password",
                changeDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion



        public void Dispose()
        {
            _client?.Dispose();
        }
    }


}