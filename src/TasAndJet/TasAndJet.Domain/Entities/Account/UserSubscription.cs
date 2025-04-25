namespace TasAndJet.Domain.Entities.Account;

public class UserSubscription
{
    private UserSubscription() { }

    private UserSubscription(Guid id, Guid userId, string stripeSubscriptionId, DateTime startDate, DateTime? endDate)
    {
        Id = id;
        UserId = userId;
        StripeSubscriptionId = stripeSubscriptionId;
        StartDate = startDate;
        EndDate = endDate;
    }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string StripeSubscriptionId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    
    public void SetEndDate(DateTime endDate)
    {
        EndDate ??= endDate;
    }

    public bool IsPremium()
    {
        return EndDate.HasValue && EndDate.Value >= DateTime.UtcNow;
    }
    
    public static UserSubscription Create(Guid id, Guid userId, string stripeSubscriptionId, DateTime startDate, DateTime? endDate)
    {
        return new UserSubscription(id, userId, stripeSubscriptionId, startDate, endDate);
    }
}
