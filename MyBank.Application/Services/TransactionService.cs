using CSharpFunctionalExtensions;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Interfaces;
using MyBank.Domain.Services; 
namespace MyBank.Application.Services;

public class TransactionService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferDomainService _transferDomainService;

    public TransactionService(
        IAccountRepository accountRepository, 
        ITransactionRepository transactionRepository, 
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _transferDomainService = new TransferDomainService();
    }

    public async Task<Result<TransactionResponse>> TransferAsync(CreateTransferRequest request, CancellationToken ct)
    {
        var fromAccount = await _accountRepository.GetByIdAsync(request.FromAccountId, ct);
        var toAccount = await _accountRepository.GetByIdAsync(request.ToAccountId, ct);

        if (fromAccount == null || toAccount == null)
            return Result.Failure<TransactionResponse>("One of the accounts was not found");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var transferResult = _transferDomainService.Transfer(
                fromAccount, toAccount, request.Amount, request.Description);

            if (transferResult.IsFailure)
                return Result.Failure<TransactionResponse>(transferResult.Error);

            var transactionEntity = transferResult.Value;

            await _transactionRepository.AddAsync(transactionEntity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success(new TransactionResponse(
                transactionEntity.Id,
                transactionEntity.FromAccountId!.Value,
                transactionEntity.ToAccountId!.Value,
                transactionEntity.Amount,
                transactionEntity.TransactionType.ToString(),
                transactionEntity.Status.ToString(),
                transactionEntity.Description,
                transactionEntity.CreatedAt
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<TransactionResponse>($"{ex.Message}");
        }
    }

    public async Task<List<TransactionResponse>> GetAccountHistoryAsync(Guid accountId, CancellationToken ct)
    {
        var transactions = await _transactionRepository.GetByAccountIdAsync(accountId, ct);
        
        var response = new List<TransactionResponse>();
        foreach (var t in transactions)
        {
            response.Add(new TransactionResponse(
                t.Id,
                t.FromAccountId ?? Guid.Empty,
                t.ToAccountId ?? Guid.Empty,
                t.Amount,
                t.TransactionType.ToString(),
                t.Status.ToString(),
                t.Description,
                t.CreatedAt
            ));
        }
        return response;
    }
}