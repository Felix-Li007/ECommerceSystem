namespace UserService.Tests.Unit.Domain
{
    using System;
    using FluentAssertions;
    using UserService.Domain.Entities;
    using Xunit;

    public class UserTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var userName = "testuser";
            var email = "testuser@example.com";
            var passwordHash = "hashedpassword";
            var fullName = "Test User";
            var phoneNumber = "1234567890";

            // Act
            var user = new User(userName, email, passwordHash, fullName, phoneNumber);
            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBeEmpty();
            user.Username.Should().Be(userName);
            user.Email.Should().Be(email);
            user.PasswordHash.Should().Be(passwordHash);
            user.FullName.Should().Be(fullName);
            user.PhoneNumber.Should().Be(phoneNumber);
            user.Status.Should().Be(UserStatus.Active);
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_WithNullUsername_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new User(null!, "testuser@example.com", "hashedpassword", "Test User", "1234567890");
            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_WithNullEmail_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new User("username", null!, "hash", "Full Name");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("email");
        }

        [Fact]
        public void Constructor_WithNullPasswordHash_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new User("username", "email@test.com", null!, "Full Name");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("passwordHash");
        }

        [Fact]
        public void UpdateProfile_WithValidData_ShouldUpdateUserInfo()
        {
            // Arrange
            var user = new User("user", "user@test.com", "hash", "Original Name");
            var newFullName = "Updated Name";
            var newPhoneNumber = "9876543210";

            // Act
            user.UpdateProfile(newFullName, newPhoneNumber);

            // Assert
            user.FullName.Should().Be(newFullName);
            user.PhoneNumber.Should().Be(newPhoneNumber);
            // user.UpdatedAt.Should().NotBeNull();
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void UpdateProfile_WithNullFullName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = new User("user", "user@test.com", "hash", "Original Name");

            // Act
            Action act = () => user.UpdateProfile(null!, "1234567890");

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangePassword_WithValidPassword_ShouldUpdatePassword()
        {
            // Arrange
            var user = new User("user", "user@test.com", "oldhash", "User Name");
            var newPasswordHash = "newhash";

            // Act
            user.ChangePassword(newPasswordHash);

            // Assert
            user.PasswordHash.Should().Be(newPasswordHash);
            // user.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Activate_ShouldSetStatusToActive()
        {
            // Arrange
            var user = new User("user", "user@test.com", "hash", "User Name");
            user.Deactivate(); // First deactivate

            // Act
            user.Activate();

            // Assert
            user.Status.Should().Be(UserStatus.Active);
            // user.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Deactivate_ShouldSetStatusToInactive()
        {
            // Arrange
            var user = new User("user", "user@test.com", "hash", "User Name");

            // Act
            user.Deactivate();

            // Assert
            user.Status.Should().Be(UserStatus.Inactive);
            // user.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Suspend_ShouldSetStatusToSuspended()
        {
            // Arrange
            var user = new User("user", "user@test.com", "hash", "User Name");

            // Act
            user.Suspend();

            // Assert
            user.Status.Should().Be(UserStatus.Suspended);
            // user.UpdatedAt.Should().NotBeNull();
        }
    }
}