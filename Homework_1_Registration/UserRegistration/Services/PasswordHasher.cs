using System.Security.Cryptography;

namespace UserRegistration.Services;

public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public (string Hash, string Salt) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return (
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt)
        );
    }

    public bool VerifyPassword(
        string password,
        string storedHash,
        string storedSalt)
    {
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] expectedHash = Convert.FromBase64String(storedHash);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash);
    }
}

