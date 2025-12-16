using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.DTOs.Analytics;
using MyBank.Infrastructure.Persistence;
using Npgsql;

namespace MyBank.Application.Services;

public class ReportService
{
    private readonly BankDbContext _context;

    public ReportService(BankDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<List<UserLoanReportDto>>> GetUserLoanReportAsync(CancellationToken cancellationToken = default)
    {
        try
        { 
            var sql = @"
                SELECT 
                    u.""Id"" as UserId,
                    u.""FirstName"",
                    u.""LastName"",
                    u.""Email"",
                    COUNT(l.""Id"") as TotalLoans, 
                    SUM(l.""PrincipalAmount"" - l.""PaidAmount"") as RemainingDebt
                FROM ""Users"" u
                JOIN ""Loans"" l ON u.""Id"" = l.""UserId"" AND l.""Status"" = 'Active'
                WHERE NOT u.""IsDeleted"" AND NOT l.""IsDeleted""
                GROUP BY u.""Id"", u.""FirstName"", u.""LastName"", u.""Email""
                ORDER BY RemainingDebt DESC";

            var results = await _context.Database
                .SqlQueryRaw<UserLoanReportDto>(sql)
                .ToListAsync(cancellationToken);

            return Result.Success(results);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<UserLoanReportDto>>(
                $"{ex.Message}");
        }
    }
    
    public async Task<Result<List<MonthlyTransactionReportDto>>> GetMonthlyTransactionReportAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = @"
            SELECT 
                a.""Currency"" as Currency,
                DATE_TRUNC('month', t.""CreatedAt"") as Month,
                COUNT(t.""Id"") as TransactionCount,
                SUM(t.""Amount"") as TotalVolume,
                AVG(t.""Amount"") as AverageTransaction
            FROM ""Transactions"" t
            JOIN ""Accounts"" a ON t.""FromAccountId"" = a.""Id""
            WHERE t.""Status"" = 'Completed'
            AND t.""TransactionType"" = 'Transfer'
            AND a.""UserId"" = {0}
            GROUP BY a.""Currency"", DATE_TRUNC('month', t.""CreatedAt"")
            ORDER BY Month DESC";

            var results = await _context.Database
                .SqlQueryRaw<MonthlyTransactionReportDto>(sql, userId)
                .ToListAsync(cancellationToken);

            return Result.Success(results);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<MonthlyTransactionReportDto>>($"{ex.Message}");
        }
    }
}