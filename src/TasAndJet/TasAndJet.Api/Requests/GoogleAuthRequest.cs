using TasAndJet.Contracts.Dto;

namespace TasAndJet.Api.Requests;

public record GoogleAuthRequest(string GoogleToken, string PhoneNumber, string Region, string Address,int RoleId);