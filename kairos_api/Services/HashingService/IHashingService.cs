namespace kairos_api.Services.HashingService;

public interface IHashingService
{
    string HashPassword(string password, string salt);
    bool VerifyPassword(string password, string hashedPassword, string salt);
}
