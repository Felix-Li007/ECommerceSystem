namespace UserService.Tests.Unit.Infrastructure
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using UserService.Domain.Entities;
    using UserService.Infrastructure.Data;
    using UserService.Infrastructure.Repositories;
    using Xunit;

    public class UserRepositoryTests : IDisposable
    {
        private readonly UserDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new UserDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");

            // Act
            var result = await _repository.AddAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();

            var savedUser = await _context.Users.FindAsync(result.Id);
            savedUser.Should().NotBeNull();
            savedUser!.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.GetByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.GetByEmailAsync("test@example.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetByUsernameAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.GetByUsernameAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result!.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var user1 = new User("user1", "user1@example.com", "hash", "User 1");
            var user2 = new User("user2", "user2@example.com", "hash", "User 2");
            await _repository.AddAsync(user1);
            await _repository.AddAsync(user2);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Username == "user1");
            result.Should().Contain(u => u.Username == "user2");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Original Name");
            await _repository.AddAsync(user);

            // Act
            user.UpdateProfile("Updated Name", "1234567890");
            await _repository.UpdateAsync(user);

            // Assert
            var updatedUser = await _repository.GetByIdAsync(user.Id);
            updatedUser!.FullName.Should().Be("Updated Name");
            updatedUser.PhoneNumber.Should().Be("1234567890");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            await _repository.AddAsync(user);
            var userId = user.Id;

            // Act
            await _repository.DeleteAsync(userId);

            // Assert
            var deletedUser = await _repository.GetByIdAsync(userId);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task ExistsEmailAsync_WhenEmailExists_ShouldReturnTrue()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.ExistsEmailAsync("test@example.com");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsEmailAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.ExistsEmailAsync("nonexistent@example.com");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsUsernameAsync_WhenUsernameExists_ShouldReturnTrue()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hash", "Test User");
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.ExistsUsernameAsync("testuser");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsUsernameAsync_WhenUsernameDoesNotExist_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.ExistsUsernameAsync("nonexistentuser");

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}