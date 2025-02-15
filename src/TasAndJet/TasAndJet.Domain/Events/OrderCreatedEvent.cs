using SharedKernel.Domain;

namespace TasAndJet.Domain.Events;

public record OrderCreatedEvent(Guid OrderId, decimal Amount) : IDomainEvent;