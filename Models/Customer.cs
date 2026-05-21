using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

// Customer extends User with customer-specific data: phone and address.
// Role is hardcoded to "Customer" in the constructor so it can never be set to something else accidentally.
public class Customer : User
{
    private int phoneNumber;
    private string? address;

    public int PhoneNumber
    {
        get => phoneNumber;
        set => phoneNumber = value;
    }

    // Address is optional — a customer may not have one registered yet.
    public string? Address
    {
        get => address;
        set => address = value;
    }

    public Customer(int userId, string firstName, string lastName, string email, string password
        , int phoneNumber, string? address)
        : base(userId, firstName, lastName, email, password)
    {
        PhoneNumber   = phoneNumber;
        Address       = address;
        // Force the role here so a Customer object can never have a different role.
        Role          = "Customer";
    }

    // Account creation is a two-step DB operation (insert User then insert Customer).
    public static async Task<Customer> CreateAccountAsync(string firstName, string lastName, string email, string password, string? phoneNumber, string? address, ICustomerDAL dal)
        => await dal.CreateAccountAsync(firstName, lastName, email, password, phoneNumber, address);

    // Returns true if an account already exists with this email
    public static async Task<bool> GetByEmailAsync(string email, ICustomerDAL dal)
        => await dal.GetByEmailAsync(email);

    public static async Task<Customer> GetByIdAsync(int customerId, ICustomerDAL dal)
        => await dal.GetByIdAsync(customerId);

    public async Task UpdateAsync(string firstName, string lastName, int phoneNumber, string? address, string password, ICustomerDAL dal)
        => await dal.UpdateAsync(UserId, firstName, lastName, phoneNumber, address, password);
}
