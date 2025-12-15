# Documentation Of Analytical Queries

## Query 1: Monthly statistic of user

**Business-question: Does the user's monthly transaction volume exhibit irregular spikes or exceed standard money flow**

With this, we can determine what traffic passes through the user monthly, and in which months it is especially active.

### SQL-QUERY
```sql

SELECT 

                a.""Currency"" as Currency,
                DATE_TRUNC('month', t.""CreatedAt"") as Month,
                COUNT(DISTINCT t.""Id"") as TransactionCount,
                SUM(t.""Amount"") as TotalVolume,
                AVG(t.""Amount"") as AverageTransaction
            FROM ""Transactions"" t
            JOIN ""Accounts"" a ON t.""FromAccountId"" = a.""Id""
            WHERE t.""Status"" = 'Completed'
            AND t.""TransactionType"" = 'Transfer'
            GROUP BY a.""Currency"", DATE_TRUNC('month', t.""CreatedAt"")
            ORDER BY Month DESC, TotalVolume DESC";

```

## Explanation of logic

+ **JOIN (Inner Join): connects the Transactions table with the Accounts table.**
+ **Where: t.""Status"" = 'Completed': Filters out failed, pending, or cancelled transactions.<br>t.""TransactionType"" = 'Transfer': Focuses specifically on money transfers.**
+ **DATE_TRUNC: This function takes the exact timestamp of a transaction and "truncates" it to the first day of that month. So it allows the GROUP BY clause to get all transactions from "Month" together**
#### **Aggregate Functions:**
+ **COUNT: Counts the total number of unique transactions in that month.**
+ **SUM: Calculates the total volume of money sent.**
+ **AVG: Calculates the average size of a single transaction**
#### Ordering (ORDER BY):
- **Month DESC: Shows the most recent activity first.**
- **TotalVolume DESC: Lists the currencies with the highest volume first, highlighting where the most money is moving.**
## Example result

## Query 2: User Active Loan & Risk Report
### Business-question: 
Which users currently hold active loans, what is their total outstanding debt, and who poses the highest financial risk?
With this, we can identify debtors, assess the total remaining debt per user, and categorize them by risk level based on the amount owed.
### SQL-QUERY
```sql
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
ORDER BY RemainingDebt DESC
```

### Explanation of logic
#### Inner Join: Connects the Users table with the Loans table.

#### Condition l.""Status"" = 'Active': Ensures we only consider currently active loans, ignoring already paid or closed ones.

#### WHERE: NOT u.""IsDeleted"" AND NOT l.""IsDeleted"": Excludes soft-deleted records to maintain data integrity and report only on existing users and valid loans.

### Aggregate Functions:
#### COUNT(l.""Id""): Counts the number of active loans for each specific user.

#### SUM(l.""PrincipalAmount"" - l.""PaidAmount""): Calculates the Remaining Debt by subtracting the amount already paid from the original loan amount for all active loans.

### Grouping & Ordering:
#### GROUP BY: Groups the results by unique user identifiers (Id, Name, Email) to perform calculations per user.

#### ORDER BY RemainingDebt DESC: Lists users with the highest debt first, immediately highlighting the highest financial risks.

## Example result

