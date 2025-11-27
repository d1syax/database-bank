using CSharpFunctionalExtensions;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;
using MyBank.Domain.Interfaces;

namespace MyBank.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IAccountRepository accountRepository, ICardRepository cardRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<UserResponse>> GetProfileAsync(Guid id, CancellationToken ct)
    {
        var user = await 
            _userRepository.GetByIdAsync(id, ct);
        if (user == null) 
            return Result.Failure<UserResponse>("User not found");

        return Result.Success(new UserResponse(
            user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.CreatedAt
        ));
    }

    public async Task<Result<UserResponse>> RegisterAsync(CreateUserRequest request, CancellationToken ct)
    {
        if (await _userRepository.IsEmailExists(request.Email, ct))
            return Result.Failure<UserResponse>("Email already exists");

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var userResult = UserEntity.Create(
                request.FirstName, request.LastName, request.Email,
                request.Password, request.DateOfBirth, request.Phone);

            if (userResult.IsFailure) 
                return Result.Failure<UserResponse>(userResult.Error);

            var user = userResult.Value;
            await _userRepository.AddAsync(user, ct);

            var accountResult = AccountEntity.Create(user.Id, "UAH", AccountType.Debit);
            if (accountResult.IsFailure) 
                return Result.Failure<UserResponse>(accountResult.Error);

            var account = accountResult.Value;
            await _accountRepository.AddAsync(account, ct);

            var cardResult = CardEntity.Create(account.Id, CardType.Debit);
            if (cardResult.IsFailure)
                return Result.Failure<UserResponse>(cardResult.Error);

            var card = cardResult.Value;
            await _cardRepository.AddAsync(card, ct);
            
            await _unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success(new UserResponse(
                user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.CreatedAt
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<UserResponse>($"{ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> UpdateProfileAsync(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null) 
            return Result.Failure<UserResponse>("User not found");

        var result = user.UpdateProfile(request.FirstName, request.LastName, request.Phone);
        if (result.IsFailure) 
            return Result.Failure<UserResponse>(result.Error);

        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new UserResponse(
            user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.CreatedAt
        ));
    }

    public async Task<Result> DeleteUserAsync(Guid id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null) 
            return Result.Failure("User not found");

        await _userRepository.DeleteAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}