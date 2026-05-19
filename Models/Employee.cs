namespace ClickAndGoApp.Models;

public abstract class Employee : User
{
    private Store store;

    public Store Store
    {
        get => store;
        set => store = value ?? throw new ArgumentNullException("Store cannot be null");
    }

    public Employee(int userId, string firstName, string lastName,
        string email, string password, Store store)
        : base(userId, firstName, lastName, email, password)
    {
        Store = store;
    }
}
