using kairos_api.DTOs.CapsuleDTOs;
using kairos_api.Entities;
using kairos_api.Repositories;
using kairos_api.Services.CapsuleService;
using kairos_api.Services.GeolocationService;
using Moq;

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
            .Returns(capsules.AsQueryable());
        
        // Act
        var result = await _capsuleService.GetCapsulesForUserAsync(user);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
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

        var capsules = new List<Capsule> { capsule }.AsQueryable();

        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(capsules);

        // Mock the async behavior manually (if CapsuleService uses FirstOrDefaultAsync internally)
        _unitOfWorkMock.Setup(u => u.Capsules.GetQueryable())
            .Returns(() => capsules.Provider.CreateQuery<Capsule>(
                capsules.Expression).AsQueryable());

        var dto = new GetCapsuleDTO { Latitude = 0, Longitude = 0 };

        // Act
        var result = await _capsuleService.GetCapsuleForUserAsync(user, capsuleId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(capsuleId, result.Id);
    }
}
