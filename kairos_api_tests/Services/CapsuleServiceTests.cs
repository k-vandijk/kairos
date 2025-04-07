using kairos_api.DTOs.CapsuleDTOs;
using kairos_api.Entities;
using kairos_api.Repositories;
using kairos_api.Services.CapsuleService;
using kairos_api.Services.GeolocationService;
using Moq;
using MockQueryable;
using Xunit;

namespace kairos_api_tests.Services;

public class CapsuleServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGeolocationService> _geolocationServiceMock;
    private readonly CapsuleService _capsuleService;

    public CapsuleServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _geolocationServiceMock = new Mock<IGeolocationService>();

        _capsuleService = new CapsuleService(
            _unitOfWorkMock.Object,
            _geolocationServiceMock.Object);
    }

    [Fact]
    public async Task GetCapsulesForUserAsync_ShouldReturnCapsules()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsules = new List<Capsule>
        {
            new Capsule { Id = Guid.NewGuid(), UserId = user.Id, DateToOpen = DateTime.Now.AddDays(-1), Latitude = 0, Longitude = 0 },
            new Capsule { Id = Guid.NewGuid(), UserId = user.Id, DateToOpen = DateTime.Now.AddDays(-2), Latitude = 0, Longitude = 0 }
        };
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());

        // Act
        var result = await _capsuleService.GetCapsulesForUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCapsulesForUserAsync_ShouldReturnEmptyList_WhenNoCapsules()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsules = new List<Capsule>();
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());
        
        // Act
        var result = await _capsuleService.GetCapsulesForUserAsync(user);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCapsuleForUserAsync_ShouldReturnCapsule()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsuleId = Guid.NewGuid();
        var capsule = new Capsule
        {
            Id = capsuleId,
            UserId = user.Id,
            DateToOpen = DateTime.Now.AddDays(-1),
            Latitude = 0,
            Longitude = 0
        };

        var capsules = new List<Capsule> { capsule };

        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());

        var dto = new GetCapsuleDTO { Latitude = 0, Longitude = 0 };

        // Act
        var result = await _capsuleService.GetCapsuleForUserAsync(user, capsuleId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(capsuleId, result.Id);
    }

    [Fact]
    public async Task GetCapsuleForUserAsync_ShouldThrowKeyNotFoundException_WhenCapsuleNotFound()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsuleId = Guid.NewGuid();
        var capsules = new List<Capsule>();
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());
        var dto = new GetCapsuleDTO { Latitude = 0, Longitude = 0 };
        
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _capsuleService.GetCapsuleForUserAsync(user, capsuleId, dto));
    }

    [Fact]
    public async Task GetCapsuleForUserAsync_ShouldThrowInvalidOperationException_WhenCapsuleIsNotOpen()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsuleId = Guid.NewGuid();
        var capsule = new Capsule
        {
            Id = capsuleId,
            UserId = user.Id,
            DateToOpen = DateTime.Now.AddDays(1),
            Latitude = 0,
            Longitude = 0
        };
        var capsules = new List<Capsule> { capsule };
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());
        var dto = new GetCapsuleDTO { Latitude = 0, Longitude = 0 };
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _capsuleService.GetCapsuleForUserAsync(user, capsuleId, dto));
    }

    [Fact]
    public async Task GetCapsuleForUserAsync_ShouldThrowInvalidOperationException_WhenCapsuleIsOutOfRange()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsuleId = Guid.NewGuid();
        var capsule = new Capsule
        {
            Id = capsuleId,
            UserId = user.Id,
            DateToOpen = DateTime.Now.AddDays(-1),
            Latitude = 0,
            Longitude = 0
        };
        var capsules = new List<Capsule> { capsule };
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());

        var dto = new GetCapsuleDTO { Latitude = 100, Longitude = 100 };
        _geolocationServiceMock.Setup(g => g.CalculateDistance(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns(10000); // Simulate out of range
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _capsuleService.GetCapsuleForUserAsync(user, capsuleId, dto));
    }

    [Fact]
    public async Task GetCapsuleForUserAsync_ShouldNotThrow_WhenCapsuleIsInRange()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsuleId = Guid.NewGuid();
        var capsule = new Capsule
        {
            Id = capsuleId,
            UserId = user.Id,
            DateToOpen = DateTime.Now.AddDays(-1),
            Latitude = 0,
            Longitude = 0
        };
        var capsules = new List<Capsule> { capsule };
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules.AsQueryable().BuildMock());
        var dto = new GetCapsuleDTO { Latitude = 0, Longitude = 0 };
        _geolocationServiceMock.Setup(g => g.CalculateDistance(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns(500); // Simulate in range

        // Act
        var result = await _capsuleService.GetCapsuleForUserAsync(user, capsuleId, dto);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateCapsuleForUserAsync_ShouldCreateCapsule()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var capsuleDto = new CreateCapsuleDTO
        {
            DateToOpen = DateTime.Now.AddDays(1),
            Latitude = 0,
            Longitude = 0
        };
        _unitOfWorkMock.Setup(u => u.Capsules.AddAsync(It.IsAny<Capsule>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await _capsuleService.CreateCapsuleForUserAsync(user, capsuleDto);
     
        // Assert
        _unitOfWorkMock.Verify(u => u.Capsules.AddAsync(It.IsAny<Capsule>()), Times.Once);
    }
}