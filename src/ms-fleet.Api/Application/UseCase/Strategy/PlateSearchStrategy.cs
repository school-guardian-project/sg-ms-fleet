using System.Text.RegularExpressions;
using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase.Strategy;

public class PlateSearchStrategy : IBusSearchStrategy
{
    private static readonly Regex PlateShape =
        new("^(?:[A-Za-z]{3}|[0-9]{3})[- ]?[0-9]{3}$", RegexOptions.Compiled);

    public bool CanHandle(string search)
    {
        return PlateShape.IsMatch(search);
    }

    public bool Matches(BusListItemDto bus, string search)
    {
        return bus.Plate.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
}
