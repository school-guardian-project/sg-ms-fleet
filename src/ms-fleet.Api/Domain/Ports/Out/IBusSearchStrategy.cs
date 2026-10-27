using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.Out;

public interface IBusSearchStrategy
{
    bool CanHandle(string search);

    bool Matches(BusListItemResponse bus, string search);
}