using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserRegistration.Data;
using UserRegistration.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString =
    configuration.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(connectionString)
    .Options;

using var context = new AppDbContext(options);

var passwordHasher = new PasswordHasher();

var userService = new UserService(
    context,
    passwordHasher);

await context.Database.MigrateAsync();

while (true)
{
    Console.Clear();

    Console.WriteLine("=== Управление пользователями ===");
    Console.WriteLine("1. Регистрация");
    Console.WriteLine("2. Авторизация");
    Console.WriteLine("3. Создать пользователя");
    Console.WriteLine("4. Редактировать пользователя");
    Console.WriteLine("5. Удалить пользователя");
    Console.WriteLine("6. Найти пользователя по ID");
    Console.WriteLine("7. Список пользователей");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите действие: ");

    string? choice = Console.ReadLine();

    Console.Clear();

    switch (choice)
    {
        case "1":
            await Register(userService);
            break;

        case "2":
            await Login(userService);
            break;

        case "3":
            await CreateUser(userService);
            break;

        case "4":
            await UpdateUser(userService);
            break;

        case "5":
            await DeleteUser(userService);
            break;

        case "6":
            await FindUserById(userService);
            break;

        case "7":
            await ShowUsers(userService);
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Неизвестная команда.");
            break;
    }

    Console.WriteLine();
    Console.WriteLine("Нажмите Enter...");
    Console.ReadLine();
}

#region Actions
static async Task Register(UserService userService)
{
    Console.WriteLine("=== Регистрация ===");

    Console.Write("Логин: ");
    string login = Console.ReadLine()!;

    Console.Write("Пароль: ");
    string password = Console.ReadLine()!;

    Console.Write("Имя: ");
    string? name = Console.ReadLine();

    Console.Write("Email: ");
    string? email = Console.ReadLine();

    bool result = await userService.CreateUserAsync(
        login,
        password,
        name,
        email);

    Console.WriteLine(
        result
            ? "Пользователь успешно зарегистрирован."
            : "Пользователь с таким логином уже существует.");
}

static async Task Login(UserService userService)
{
    Console.WriteLine("=== Авторизация ===");

    Console.Write("Логин: ");
    string login = Console.ReadLine()!;

    Console.Write("Пароль: ");
    string password = Console.ReadLine()!;

    var user = await userService.AuthenticateAsync(
        login,
        password);

    if (user == null)
    {
        Console.WriteLine("Неверный логин или пароль.");
        return;
    }

    Console.WriteLine(
        $"Добро пожаловать, {user.Name ?? user.Login}!");
}

static async Task CreateUser(UserService userService)
{
    Console.WriteLine("=== Создание пользователя ===");

    Console.Write("Логин: ");
    string login = Console.ReadLine()!;

    Console.Write("Пароль: ");
    string password = Console.ReadLine()!;

    Console.Write("Имя: ");
    string? name = Console.ReadLine();

    Console.Write("Email: ");
    string? email = Console.ReadLine();

    bool result = await userService.CreateUserAsync(
        login,
        password,
        name,
        email);

    Console.WriteLine(
        result
            ? "Пользователь создан."
            : "Такой логин уже занят.");
}

static async Task UpdateUser(UserService userService)
{
    Console.WriteLine("=== Редактирование пользователя ===");

    Console.Write("ID пользователя: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Некорректный ID.");
        return;
    }

    var user = await userService.GetUserAsync(id);

    if (user == null)
    {
        Console.WriteLine("Пользователь не найден.");
        return;
    }

    Console.Write($"Имя ({user.Name}): ");
    string? name = Console.ReadLine();

    Console.Write($"Email ({user.Email}): ");
    string? email = Console.ReadLine();

    Console.Write("Новый пароль (Enter — оставить старый): ");
    string? newPassword = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(name))
        name = user.Name;

    if (string.IsNullOrWhiteSpace(email))
        email = user.Email;

    bool result = await userService.UpdateUserAsync(
        id,
        name,
        email,
        newPassword);

    Console.WriteLine(
        result
            ? "Пользователь обновлён."
            : "Ошибка обновления.");
}

static async Task DeleteUser(UserService userService)
{
    Console.WriteLine("=== Удаление пользователя ===");

    Console.Write("ID пользователя: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Некорректный ID.");
        return;
    }

    var user = await userService.GetUserAsync(id);

    if (user == null)
    {
        Console.WriteLine("Пользователь не найден.");
        return;
    }

    Console.Write(
        $"Удалить пользователя {user.Login}? (y/n): ");

    string? confirmation = Console.ReadLine();

    if (confirmation?.ToLower() != "y")
    {
        Console.WriteLine("Удаление отменено.");
        return;
    }

    bool result = await userService.DeleteUserAsync(id);

    Console.WriteLine(
        result
            ? "Пользователь удалён."
            : "Ошибка удаления.");
}

static async Task FindUserById(UserService userService)
{
    Console.WriteLine("=== Поиск пользователя ===");

    Console.Write("Введите ID: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Некорректный ID.");
        return;
    }

    var user = await userService.GetUserByIdAsync(id);

    if (user == null)
    {
        Console.WriteLine("Пользователь не найден.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"ID:         {user.Id}");
    Console.WriteLine($"Логин:      {user.Login}");
    Console.WriteLine($"Имя:        {user.Name ?? "-"}");
    Console.WriteLine($"Email:      {user.Email ?? "-"}");
    Console.WriteLine($"Создан:     {user.CreatedAt:dd.MM.yyyy HH:mm}");
}
static async Task ShowUsers(UserService userService)
{
    Console.WriteLine("=== Список пользователей ===");

    var users = await userService.GetAllUsersAsync();

    if (users.Count == 0)
    {
        Console.WriteLine("Пользователей пока нет.");
        return;
    }

    Console.WriteLine();

    Console.WriteLine(
        "{0,-5} {1,-20} {2,-25} {3,-20}",
        "ID",
        "Логин",
        "Email",
        "Имя");

    Console.WriteLine(new string('-', 75));

    foreach (var user in users)
    {
        Console.WriteLine(
            "{0,-5} {1,-20} {2,-25} {3,-20}",
            user.Id,
            user.Login,
            user.Email ?? "-",
            user.Name ?? "-");
    }
}
#endregion