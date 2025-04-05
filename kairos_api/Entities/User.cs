namespace kairos_api.Entities;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

    public virtual IEnumerable<Capsule> Capsules { get; set; }
}
