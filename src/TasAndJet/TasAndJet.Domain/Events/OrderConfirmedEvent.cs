using SharedKernel.Domain;

namespace TasAndJet.Domain.Events;

public record OrderConfirmedEvent(Guid OrderId) : IDomainEvent;