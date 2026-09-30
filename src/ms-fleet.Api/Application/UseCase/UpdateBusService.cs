using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class UpdateBusService : IUpdateBusUseCase
{
    private readonly IBusRepository _busRepository;

    public UpdateBusService(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public async Task ExecuteAsync(
        Guid busId,
        Guid campuseId,
        DateTime soatValidity,
        byte capacity,
        int modelId,
        CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");

        bus.CampuseId = campuseId;
        bus.SoatValidity = soatValidity;
        bus.Capacity = capacity;
        bus.ModelId = (byte)modelId;

        await _busRepository.UpdateAsync(bus, ct);
    }
}
