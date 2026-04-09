namespace SMCV.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserProfile? UserProfile { get; set; }
    public ICollection<Campaign> Campaigns { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Campaigns = new List<Campaign>();
    }

    protected User(bool _)
    {
        Campaigns = new List<Campaign>();
    }
}
