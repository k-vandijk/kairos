namespace API.Services.HashingService;

public class HashingService : IHashingService
{
    private readonly string _pepper;
    public HashingService()
    {
        _pepper = Environment.GetEnvironmentVariable("PEPPER")!;
    }

    public string HashPassword(string password, string salt)
    {
        string pepperedPassword = password + _pepper;
        return BCrypt.Net.BCrypt.HashPassword(pepperedPassword, salt);
    }

    public bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        string pepperedPassword = password + _pepper;
        string hashedPepperedPassword = BCrypt.Net.BCrypt.HashPassword(pepperedPassword, salt);

        return hashedPassword == hashedPepperedPassword;
    }
}