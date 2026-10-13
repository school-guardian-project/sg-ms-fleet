using NUnit.Framework;
namespace ms_fleet.Api.Domain.Model;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class BusTests
{
    [Test]
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

        Assert.That(bus.Status, Is.EqualTo(Status.Active));
        Assert.That(bus.Plate, Is.EqualTo("ABC123"));
        Assert.That(bus.Capacity, Is.EqualTo(40));
    }

    [Test]
    public void CreateBus_ShouldGenerateNewId()
    {
        var bus = new Bus();
        Assert.That(bus.Id, Is.Not.EqualTo(Guid.Empty));
    }
}
