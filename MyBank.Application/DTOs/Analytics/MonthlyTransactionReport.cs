namespace MyBank.Application.DTOs.Analytics;

public class MonthlyTransactionReportDto
{
    public string Currency { get; set; } = string.Empty;
    public DateTime Month { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal AverageTransaction { get; set; }
}