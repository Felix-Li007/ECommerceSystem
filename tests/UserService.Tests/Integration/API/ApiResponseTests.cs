namespace UserService.Tests.Integration.API
{


    using System.Net;
    using System.Net.Http.Json;
    using Microsoft.AspNetCore.Mvc.Testing;
    using FluentAssertions;
    using Xunit;

    public class ApiResponseTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiResponseTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ApiResponse_ShouldHaveCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            response.Content.Headers.ContentType?.MediaType
                .Should().Be("application/json");
        }



        [Fact]
        public async Task ErrorResponse_ShouldHaveConsistentFormat()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/users/{nonExistentId}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Contain("detail");
        }

        [Fact]
        public async Task ValidationError_ShouldReturn400WithDetails()
        {
            // Arrange
            var invalidData = new { Username = "" }; // Missing required fields

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", invalidData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}