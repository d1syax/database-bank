using CSharpFunctionalExtensions;
using MyBank.Domain.Common;

namespace MyBank.Domain.Entities;

public class UserEntity : SoftDeletableEntity
{
    private UserEntity() { }
    public UserEntity(string firstName, string lastName, string email, string passwordHash, DateTime dateOfBirth, string phoneNumber)
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

    public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
    public ICollection<LoanEntity> Loans { get; set; } = new List<LoanEntity>();
    
    public string FullName => $"{FirstName} {LastName}";
    
    public int Age
    {
        get
        {
            var today = DateTime.UtcNow;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public static Result<UserEntity> Create(string firstName, string lastName, string email, string passwordHash, DateTime dateOfBirth, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email)) 
            return Result.Failure<UserEntity>("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(firstName)) 
            return Result.Failure<UserEntity>("Name is required");
        if (dateOfBirth > DateTime.UtcNow.AddYears(-14))
            return Result.Failure<UserEntity>("UserEntity is too young");

        var user = new UserEntity(firstName, lastName, email, passwordHash, dateOfBirth, phoneNumber);
        return Result.Success(user);
    }
    
    public Result UpdateProfile(string newFirstName, string newLastName, string newPhone)
    {
        if (string.IsNullOrWhiteSpace(newFirstName))
            return Result.Failure("Cannot be empty");
    
        if (string.IsNullOrWhiteSpace(newLastName))
            return Result.Failure("Cannot be empty");

        FirstName = newFirstName;
        LastName = newLastName;
        PhoneNumber = newPhone;

        return Result.Success();
    }
}