namespace UserService.Application.Services
{
    using DTOs;
    using Domain.Repositories;
    using Domain.Entities;
    using System.Security.Cryptography;
    using System.Text;
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // Implementation of user service methods
        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            if (await _userRepository.ExistsEmailAsync(dto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            if (await _userRepository.ExistsUsernameAsync(dto.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            var passwordHash = HashPassword(dto.Password);
            var user = new User(dto.Username, dto.Email, passwordHash, dto.FullName, dto.PhoneNumber);
            var createUser = await _userRepository.AddAsync(user);
            return MapToDto(createUser);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            user.UpdateProfile(dto.FullName, dto.PhoneNumber);
            await _userRepository.UpdateAsync(user);
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            var passwordHash = HashPassword(password);
            return user.PasswordHash == passwordHash && user.Status == UserStatus.Active;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var currentPasswordHash = HashPassword(dto.CurrentPassword);
            if (user.PasswordHash != currentPasswordHash)
            {
                return false;
            }

            var newPasswordHash = HashPassword(dto.NewPassword);
            user.ChangePassword(newPasswordHash);
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
