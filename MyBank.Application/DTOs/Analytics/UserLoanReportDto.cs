namespace MyBank.Application.DTOs.Analytics;

public class UserLoanReportDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    
    public string Email { get; set; } = string.Empty;
    public int TotalLoans { get; set; }
    public decimal RemainingDebt { get; set; }
    public string RiskCategory => RemainingDebt switch
    {
        > 50000 => "High Risk",
        > 10000 => "Medium Risk",
        _ => "Low Risk"
    };
}