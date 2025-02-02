namespace TasAndJet.Api.Entities.Reviews;
public class Review
{
    private Review() { }

    public Review(Guid id, Guid userId, string comment, int rating)
    {
        Id = id;
        UserId = userId;
        Comment = comment;
        Rating = rating;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
}
