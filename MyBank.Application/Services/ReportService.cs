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
                u.""FirstName"" || ' ' || u.""LastName"" as FullName,
                u.""Email"",
                COUNT(l.""Id"") as TotalLoans, 
                SUM(l.""PrincipalAmount"") as TotalLoanAmount,
                SUM(l.""PaidAmount"") as TotalPaid,
                SUM(l.""PrincipalAmount"" - l.""PaidAmount"") as RemainingDebt,
                CASE 
                    WHEN SUM(l.""PrincipalAmount"" - l.""PaidAmount"") > 50000 THEN 'High Risk'
                    WHEN SUM(l.""PrincipalAmount"" - l.""PaidAmount"") > 10000 THEN 'Medium Risk'
                    ELSE 'Low Risk'
                END as RiskCategory
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
}