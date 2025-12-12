namespace UserService.Application.Services
{
    using DTOs;
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto);
    }
}