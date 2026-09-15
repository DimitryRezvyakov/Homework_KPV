using Microsoft.EntityFrameworkCore;
using UserRegistration.Data;
using UserRegistration.Model;

namespace UserRegistration.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;

    public UserService(
        AppDbContext context,
        PasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> CreateUserAsync(
        string login,
        string password,
        string? name,
        string? email)
    {
        bool exists = await _context.Users
            .AnyAsync(x => x.Login == login);

        if (exists)
        {
            return false;
        }

        var passwordData = _passwordHasher.HashPassword(password);

        var user = new User
        {
            Login = login,
            PasswordHash = passwordData.Hash,
            PasswordSalt = passwordData.Salt,
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> AuthenticateAsync(
        string login,
        string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Login == login);

        if (user == null)
        {
            return null;
        }

        bool validPassword = _passwordHasher.VerifyPassword(
            password,
            user.PasswordHash,
            user.PasswordSalt);

        return validPassword ? user : null;
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateUserAsync(
        int id,
        string? name,
        string? email,
        string? newPassword)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return false;
        }

        user.Name = name;
        user.Email = email;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var passwordData =
                _passwordHasher.HashPassword(newPassword);

            user.PasswordHash = passwordData.Hash;
            user.PasswordSalt = passwordData.Salt;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

}

