using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models
{
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
            get { return userId; }
            set 
            { 
                if (value <= 0)
                    throw new ArgumentException("UserId must be positive");
                userId = value; 
            }
        }

        public string FirstName
        {
            get { return firstName; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("FirstName cannot be empty");
                firstName = value; 
            }
        }

        public string LastName
        {
            get { return lastName; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("LastName cannot be empty");
                lastName = value; 
            }
        }

        public string Email
        {
            get { return email; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty");
                if (!value.Contains("@"))
                    throw new ArgumentException("Email is not valid");
                email = value; 
            }
        }

        public string Password
        {
            get { return password; }
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
            get { return role; }
            set 
            { 
                string[] validRoles = { "Customer", "OrderPicker", "Cashier" };
                
                if (!validRoles.Contains(value))
                    throw new ArgumentException("Role is not valid");
                role = value; 
            }
        }

        public User(int userId, string firstName, string lastName,
                    string email, string password)
        {
            UserId    = userId;
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
            Password = password;
        }
        
        public User(int userId, string firstName, string lastName,
            string email, string password, string role)
            : this(userId, firstName, lastName, email, password)
        {
            Role = role;
        }
        
        //methodes
        public static Task<User> GetByCredentials(string email, string password, IUserDAL dal) => //
            dal.GetByCredentials(email, password);
    }
}