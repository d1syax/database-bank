using Microsoft.EntityFrameworkCore;
using MyBank.Infrastructure.Persistence;
using MyBank.Infrastructure.Persistence.Repositories;
using MyBank.Domain.Interfaces;

namespace MyBank.Tests;

public class TestBase : IDisposable
{
    protected readonly BankDbContext Context;
    protected readonly IUserRepository UserRepository;
    protected readonly IAccountRepository AccountRepository;
    protected readonly ICardRepository CardRepository;
    protected readonly ILoanRepository LoanRepository;
    protected readonly ITransactionRepository TransactionRepository;
    protected readonly IUnitOfWork UnitOfWork;

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new BankDbContext(options);
        
        UserRepository = new UserRepository(Context);
        AccountRepository = new AccountRepository(Context);
        CardRepository = new CardRepository(Context);
        LoanRepository = new LoanRepository(Context);
        TransactionRepository = new TransactionRepository(Context);
        UnitOfWork = new UnitOfWork(Context);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}