using FluentAssertions;
using MyBank.Domain.Entities;
using Xunit;

namespace MyBank.Tests.Domain;

public class UserEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var firstName = "Pavlo";
        var lastName = "Leheza";
        var email = "pavloleheza@gmail.com";
        var passwordHash = "hashed123";
        var dateOfBirth = new DateTime(1995, 2, 1);
        var phone = "+380991234563";

        var result = UserEntity.Create(firstName, lastName, email, passwordHash, dateOfBirth, phone);

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.Email.Should().Be(email);
        result.Value.Age.Should().BeGreaterThan(14);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldFail()
    {
        var result = UserEntity.Create("Pavlo", "Leheza", "", "hash123", new DateTime(1995, 2, 1), "+380991234563");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Email");
    }

    [Fact]
    public void Create_WithTooYoungAge_ShouldFail()
    {
        var dateOfBirth = DateTime.UtcNow.AddYears(-10); 
        
        var result = UserEntity.Create("Pavlo", "Leheza", "pavloleheza@gmail.com", "hash123", dateOfBirth, "+380991234563");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("young");
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldSucceed()
    {
        var user = UserEntity.Create("Pavlo", "Leheza", "pavloleheza@gmail.com", "hash123", new DateTime(1995, 2, 1), "+380991234563").Value;
        
        var result = user.UpdateProfile("Pavlo", "Leheza", "+380991111111");
        
        result.IsSuccess.Should().BeTrue();
        user.FirstName.Should().Be("Pavlo");
        user.LastName.Should().Be("Leheza");
        user.PhoneNumber.Should().Be("+380991111111");
    }

    [Fact]
    public void FullName_ShouldCombineFirstAndLastName()
    {
        var user = UserEntity.Create("Pavlo", "Leheza", "pavloleheza@gmail.com", "hash123", new DateTime(1995, 2, 1), "+380991234563").Value;

        var fullName = user.FullName;

        fullName.Should().Be("Pavlo Leheza");
    }
}