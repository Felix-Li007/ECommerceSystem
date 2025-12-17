namespace AdminWeb.Services;

using AdminWeb.Models.DTOs;
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto);
    Task<bool> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ToggleUserStatusAsync(int id);
    Task<List<UserDto>> SearchUsersAsync(string searchTerm);
}