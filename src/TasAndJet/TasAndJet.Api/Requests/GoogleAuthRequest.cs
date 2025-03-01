using TasAndJet.Contracts.Dto;

namespace TasAndJet.Api.Requests;

public record GoogleAuthRequest(string GoogleToken, string PhoneNumber, int RoleId, VehicleDto VehicleDto);