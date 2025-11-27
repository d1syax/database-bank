namespace MyBank.Application.DTOs.Analytics;

public class UserLoanReportDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalLoans { get; set; }
    public decimal TotalLoanAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingDebt { get; set; }
    public string RiskCategory { get; set; } = string.Empty;
}