using ms_fleet.Api.Application.Dtos;

namespace ms_fleet.Api.Domain.Ports.Out;

public interface IBusSearchStrategy
{
    bool CanHandle(string search);

    bool Matches(BusListItemDto bus, string search);
}
