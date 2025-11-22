using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class User : SoftDeletableEntity
{
    private User() { }
    public User(string firstName, string lastName, string email, string passwordHash, DateTime dateOfBirth, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
    }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    
    public string FullName => $"{FirstName} {LastName}";
    
    public int Age
    {
        get
        {
            var today = DateTime.Now;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public static Result<User> Create(string firstName, string lastName, string email, string passwordHash, DateTime dateOfBirth, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email)) return Result.Failure<User>("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(firstName)) return Result.Failure<User>("Name is required");
        if (dateOfBirth > DateTime.Now.AddYears(-14)) return Result.Failure<User>("User is too young");

        var user = new User(firstName, lastName, email, passwordHash, dateOfBirth, phoneNumber);

        return Result.Success(user);
    }
}