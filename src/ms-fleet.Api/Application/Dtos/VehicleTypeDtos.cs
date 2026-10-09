namespace ms_fleet.Api.Application.Dtos;

public record BrandListDto(byte Id, string Name);

public record ModelListDto(byte Id, string Name, byte BrandId, string BrandName);
