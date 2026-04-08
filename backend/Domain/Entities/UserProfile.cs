namespace SMCV.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string? ResumeFilePath { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserProfile()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected UserProfile(bool _) { }
}
