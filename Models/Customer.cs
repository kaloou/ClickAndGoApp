using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models;

public class Customer : User
{
    private int loyaltyPoints;
    private int phoneNumber;
    private string address;

    public int LoyaltyPoints
    {
        get => loyaltyPoints;
        set => loyaltyPoints = value >= 0
            ? value
            : throw new ArgumentException("LoyaltyPoints cannot be negative");
    }

    public int PhoneNumber
    {
        get => phoneNumber;
        set => phoneNumber = value;
    }

    public string Address
    {
        get => address;
        set => address = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Address cannot be empty");
    }

    public Customer(int userId, string firstName, string lastName, string email, string password,
                    int loyaltyPoints, int phoneNumber, string address)
        : base(userId, firstName, lastName, email, password)
    {
        LoyaltyPoints = loyaltyPoints;
        PhoneNumber   = phoneNumber;
        Address       = address;
    }
    
    //=========================
    // METHODES
    //=========================

    public static Task<bool> GetByEmail(string email, ICustomerDAL dal) =>
        dal.GetByEmail(email);

    public static Task<Customer> CreateAccount(string name, string email, string password, ICustomerDAL dal) =>
        dal.CreateAccount(name, email, password);
}
