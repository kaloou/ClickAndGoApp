using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class User
{
    private int userId;
    private string firstName;
    private string lastName;
    private string email;
    private string password;
    private string role;

    public int UserId
    {
        get => userId;
        set => userId = value > 0
            ? value
            : throw new ArgumentException("UserId must be positive");
    }

    public string FirstName
    {
        get => firstName;
        set => firstName = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("FirstName cannot be empty");
    }

    public string LastName
    {
        get => lastName;
        set => lastName = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("LastName cannot be empty");
    }

    public string Email
    {
        get => email;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty");
            if (!value.Contains("@"))
                throw new ArgumentException("Email is not valid");
            email = value;
        }

        public static User GetByCredentials(string email, string password, UserDAL dal)
            => dal.GetByCredentials(email, password);
    }

    public string Password
    {
        get => password;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password cannot be empty");
            if (value.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");
            password = value;
        }
    }

    public string Role
    {
        get => role;
        set
        {
            string[] validRoles = { "Customer", "OrderPicker", "Cashier" };
            if (!validRoles.Contains(value))
                throw new ArgumentException("Role is not valid");
            role = value;
        }
    }

    public User(int userId, string firstName, string lastName, string email, string password)
    {
        UserId    = userId;
        FirstName = firstName;
        LastName  = lastName;
        Email     = email;
        Password  = password;
    }

    public User(int userId, string firstName, string lastName, string email, string password, string role)
        : this(userId, firstName, lastName, email, password)
    {
        Role = role;
    }

    //==============================
    public static async Task<User> GetByCredentialsAsync(string email, string password, IUserDAL dal) =>
        await dal.GetByCredentialsAsync(email, password);
    
    //==============================

    public override string ToString() =>
        $"[User] Id={UserId} | {FirstName} {LastName} | {Email} | Role={Role}";

    public override bool Equals(object obj)
    {
        if (obj is not User other) return false;
        return UserId == other.UserId;
    }

    public override int GetHashCode() => UserId.GetHashCode();
}
