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
