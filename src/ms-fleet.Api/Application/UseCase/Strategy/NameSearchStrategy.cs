using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase.Strategy;

public class NameSearchStrategy : IBusSearchStrategy
{
    public bool CanHandle(string search)
    {
        return true;
    }

    public bool Matches(BusListItemResponse bus, string search)
    {
        return bus.Brand.Contains(search, StringComparison.OrdinalIgnoreCase)
            || bus.Model.Contains(search, StringComparison.OrdinalIgnoreCase)
            || bus.DriverName.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
}