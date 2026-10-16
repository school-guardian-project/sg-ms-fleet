namespace ms_fleet.Api.Infrastructure.Grpc.Dto;

/// <summary>
/// DTO para la respuesta de perfil desde ms-iam (para uso interno).
/// </summary>
public sealed record ProfileLookupDto(Guid ProfileId, string Email);