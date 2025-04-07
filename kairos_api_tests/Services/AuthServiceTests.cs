using kairos_api.DTOs.AuthDTOs;
using kairos_api.Entities;
using kairos_api.Repositories;
using kairos_api.Services.AuthService;
using kairos_api.Services.HashingService;
using kairos_api.Services.JwtTokenService;
using Moq;
using Xunit;

namespace kairos_api_tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IHashingService> _hashingServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _hashingServiceMock = new Mock<IHashingService>();

        _authService = new AuthService(
            _unitOfWorkMock.Object,
            _jwtTokenServiceMock.Object,
            _hashingServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowArgumentException_WhenPasswordIsMismatched()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Email = "kevinvd05@gmail.com",
            Password = "Test123!",
            ConfirmPassword = "test123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowArgumentException_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Email = "kevinvd05@gmail.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        // Setup the unit of work to return an existing user
        _unitOfWorkMock
            .Setup(u => u.Users.GetByEmailAsync(registerDto.Email.ToLower()))
            .ReturnsAsync(new User { Email = registerDto.Email.ToLower() });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccessMessage_WhenValidDto()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Email = "kevinvd05@gmail.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        _unitOfWorkMock
            .Setup(u => u.Users.GetByEmailAsync(registerDto.Email.ToLower()))
            .ReturnsAsync((User)null);

        _hashingServiceMock
            .Setup(h => h.HashPassword(registerDto.Password, It.IsAny<string>()))
            .Returns("hashedPassword");

        _jwtTokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("generatedToken");

        _unitOfWorkMock
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.Equal("generatedToken", result);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowArgumentException_WhenUserIsNonExistant()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "kevinvd05@gmail.com",
            Password = "Test123!"
        };

        _unitOfWorkMock
            .Setup(u => u.Users.GetByEmailAsync(loginDto.Email.ToLower()))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowArgumentException_WhenPasswordIsInvalid()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "kevinvd05@gmail.com",
            Password = "Test123!"
        };

        _unitOfWorkMock
            .Setup(u => u.Users.GetByEmailAsync(loginDto.Email.ToLower()))
            .ReturnsAsync(new User
            {
                Email = loginDto.Email.ToLower(),
                PasswordHash = "hashedPassword",
                PasswordSalt = "salt"
            });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccessMessage_WhenValidDto()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "kevinvd05@gmail.com",
            Password = "Test123!"
        };

        _unitOfWorkMock
            .Setup(u => u.Users.GetByEmailAsync(loginDto.Email.ToLower()))
            .ReturnsAsync(new User
            {
                Email = loginDto.Email.ToLower(),
                PasswordHash = "hashedPassword",
                PasswordSalt = "salt"
            });

        _hashingServiceMock
            .Setup(h => h.VerifyPassword(loginDto.Password, "hashedPassword", "salt"))
            .Returns(true);

        _jwtTokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("generatedToken");

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.Equal("generatedToken", result);
    }
}
