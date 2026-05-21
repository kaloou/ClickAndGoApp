namespace ClickAndGoApp.Models;

// Abstract intermediate class between User and the concrete employee types (OrderPicker, Cashier).
// Every employee belongs to exactly one store, which is why Store is defined here rather than in each subclass.
// The class is abstract because "Employee" alone has no meaning in our domain — you are always either a picker or a cashier.
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
