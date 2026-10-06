using Xunit;
using Xunit;
namespace ms_fleet.Api.Domain.Model;

public class BusTests
{
    [Fact]
    public void CreateBus_WithValidData_ShouldHaveActiveStatus()
    {
        var bus = new Bus
        {
            Plate = "ABC123",
            Capacity = 40,
            ModelId = 1,
            CampuseId = Guid.NewGuid(),
            GpsDeviceId = Guid.NewGuid(),
            SoatValidity = DateTime.UtcNow.AddYears(1)
        };

        Assert.Equal(Status.Active, bus.Status);
        Assert.Equal("ABC123", bus.Plate);
        Assert.Equal(40, bus.Capacity);
    }

    [Fact]
    public void CreateBus_ShouldGenerateNewId()
    {
        var bus = new Bus();
        Assert.NotEqual(Guid.Empty, bus.Id);
    }
}
