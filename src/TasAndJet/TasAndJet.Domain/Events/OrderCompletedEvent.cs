using SharedKernel.Domain;

namespace TasAndJet.Domain.Events;

public record OrderCompletedEvent(Guid DriverId) : IDomainEvent;