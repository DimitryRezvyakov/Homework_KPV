namespace UserRegistration.Model;

public class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }
}

