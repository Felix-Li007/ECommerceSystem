namespace AdminWeb.Services;

using AdminWeb.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

public class UserService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.GetAsync("/users/api/users");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserDto>>(_jsonOptions)
                    ?? new List<UserDto>();
            }

            _logger.LogError($"Failed to fetch users. Status: {response.StatusCode}");
            return new List<UserDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users from API Gateway");
            return new List<UserDto>();
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.GetAsync($"/users/api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user {id}");
            return null;
        }
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.PostAsJsonAsync("/users/api/users", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return null;
        }
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.PutAsJsonAsync($"/users/api/users/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user {id}");
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.DeleteAsync($"/users/api/users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user {id}");
            return false;
        }
    }

    public async Task<bool> ToggleUserStatusAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.PostAsync($"/users/api/users/{id}/toggle-status", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling user status {id}");
            return false;
        }
    }

    public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.GetAsync($"/users/api/users/search?term={Uri.EscapeDataString(searchTerm)}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserDto>>(_jsonOptions)
                    ?? new List<UserDto>();
            }

            return new List<UserDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching users");
            return new List<UserDto>();
        }
    }
}