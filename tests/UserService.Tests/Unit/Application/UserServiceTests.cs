namespace UserService.Tests.Unit.Application
{
    using System;
    using FluentAssertions;
    using UserService.Application.DTOs;
    using UserService.Domain.Entities;
    using Xunit;
    using Moq;
    using UserService.Domain.Repositories;

    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService.Application.Services.UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService.Application.Services.UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUserDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            typeof(User).GetProperty("Id")!.SetValue(user, userId);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Password@123",
                FullName = "New User",
                PhoneNumber = "1234567890"
            };


            _userRepositoryMock.Setup(r => r.ExistsEmailAsync(createDto.Email)).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.ExistsUsernameAsync(createDto.Username))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _userService.CreateUserAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be(createDto.Username);
            result.Email.Should().Be(createDto.Email);
            result.FullName.Should().Be(createDto.FullName);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingEmail_ShouldThrowException()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "Password@123",
                FullName = "New User"
            };

            _userRepositoryMock.Setup(r => r.ExistsEmailAsync(createDto.Email))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _userService.CreateUserAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Email already exists");
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingUsername_ShouldThrowException()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "existinguser",
                Email = "new@example.com",
                Password = "Password@123",
                FullName = "New User"
            };

            _userRepositoryMock.Setup(r => r.ExistsEmailAsync(createDto.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.ExistsUsernameAsync(createDto.Username))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _userService.CreateUserAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Username already exists");
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hash", "Old Name");
            typeof(User).GetProperty("Id")!.SetValue(user, userId);

            var updateDto = new UpdateUserDto
            {
                FullName = "New Name",
                PhoneNumber = "9876543210"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(updateDto.FullName);
            result.PhoneNumber.Should().Be(updateDto.PhoneNumber);
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FullName = "New Name"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task AuthenticateAsync_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password@123";
            var user = new User("testuser", email, HashPassword(password), "Test User");

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync(email, password);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AuthenticateAsync_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var email = "test@example.com";
            var correctPassword = "Password@123";
            var wrongPassword = "WrongPassword";
            var user = new User("testuser", email, HashPassword(correctPassword), "Test User");

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync(email, wrongPassword);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AuthenticateAsync_WithNonExistentUser_ShouldReturnFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "Password@123";

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.AuthenticateAsync(email, password);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserExists_ShouldReturnTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hash", "Test User");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.Should().BeTrue();
            _userRepositoryMock.Verify(r => r.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.Should().BeFalse();
            _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
