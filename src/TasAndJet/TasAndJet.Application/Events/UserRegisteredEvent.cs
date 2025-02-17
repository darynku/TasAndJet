namespace TasAndJet.Application.Events;

public record UserRegisteredEvent(Guid UserId, string PhoneNumber);