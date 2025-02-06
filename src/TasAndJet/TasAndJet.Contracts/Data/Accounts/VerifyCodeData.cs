namespace TasAndJet.Contracts.Data.Accounts;

public class VerifyCodeData
{
    public required string Code { get; set; }
    public required string PhoneNumber { get; set; }
}