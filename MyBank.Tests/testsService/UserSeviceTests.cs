using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.DTOs;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.Services;
using MyBank.Domain.Constants;
using Xunit;

namespace MyBank.Tests.Application;

public class UserServiceTests : TestBase
{
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(UserRepository, AccountRepository, CardRepository, UnitOfWork);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateUserWithAccountAndCard()
    {
        var request = new CreateUserRequest(
            "Pavlo",
            "Leheza",
            "pavloleheza@gmail.com",
            "+380991234563",
            "password123",
            new DateTime(1995, 5, 18)
        );

        var result = await _userService.RegisterAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("pavloleheza@gmail.com");

        var user = await UserRepository.GetByIdAsync(result.Value.Id);
        user.Should().NotBeNull();

        var accounts = await AccountRepository.GetByUserIdAsync(user!.Id);
        accounts.Should().HaveCount(1);
        accounts[0].Currency.Should().Be("UAH");
        accounts[0].Balance.Should().Be(0);

        var cards = await CardRepository.GetByAccountIdAsync(accounts[0].Id);
        cards.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldFail()
    {
        var request1 = new CreateUserRequest("Pavlo", "Leheza", "pavloleheza@gmail.com", "+380991234563", "pass123", new DateTime(1994, 4, 4));
        await _userService.RegisterAsync(request1, CancellationToken.None);

        var request2 = new CreateUserRequest("Pavlo", "Leheza", "pavloleheza@gmail.com", "+380991972357", "pass456", new DateTime(1995, 5, 5));

        var result = await _userService.RegisterAsync(request2, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }
    
    [Fact]
    public async Task UpdateProfileAsync_WithValidData_ShouldUpdateUser()
    {
        var registerRequest = new CreateUserRequest("Pavlo", "Leheza", "pavloleheza@gmail.com", "+380991234563", "pass123", new DateTime(1995, 5, 24));
        var registerResult = await _userService.RegisterAsync(registerRequest, CancellationToken.None);
        var userId = registerResult.Value.Id;

        var updateRequest = new UpdateUserRequest("Daniil", "Leheza", "+380991972357");

        var result = await _userService.UpdateProfileAsync(userId, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Daniil");
        result.Value.LastName.Should().Be("Leheza");
        result.Value.Phone.Should().Be("+380991972357");
    }
    
    [Fact]
    public async Task DeleteUserAsync_ShouldSoftDeleteUser()
    {
        var request = new CreateUserRequest("Pavlo", "Leheza", "pavloleheza@gmail.com", "+380991234563", "pass123", new DateTime(1995, 5, 24));
        var registerResult = await _userService.RegisterAsync(request, CancellationToken.None);
        var userId = registerResult.Value.Id;

        var result = await _userService.DeleteUserAsync(userId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var user = await Context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
        user.Should().NotBeNull();
        user!.IsDeleted.Should().BeTrue();
        user.DeletedAt.Should().NotBeNull();

        var deletedUser = await UserRepository.GetByIdAsync(userId);
        deletedUser.Should().BeNull();
    }
}