namespace ms_fleet.Api.Application.Dtos;

public record GpsDeviceInfo(Guid Id, string Imei, bool GpsStatus, Guid? AssignedBusId, string AssignedPlate);
