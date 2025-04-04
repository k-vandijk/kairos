namespace kairos_api.Entities;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public virtual List<Timecapsule> Timecapsules { get; set; }
}
