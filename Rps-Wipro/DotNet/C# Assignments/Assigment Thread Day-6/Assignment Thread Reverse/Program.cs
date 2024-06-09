using System;
using System.Threading;

public class Account
{
    public int AccountId { get; set; }
    public decimal AccountBalance { get; set; }
}

public class AccountManager
{
    private static readonly object LockObject = new object();

    public void Transfer(Account from, Account to, decimal amount)
    {
        lock (LockObject) // Acquire lock to ensure mutual exclusion
        {
            // Ensure sufficient balance in the 'from' account
            if (from.AccountBalance < amount)
            {
                Console.WriteLine($"Insufficient balance in Account {from.AccountId}. Transfer failed.");
            }
            else
            {
                from.AccountBalance -= amount;
                to.AccountBalance += amount;
                Console.WriteLine($"Transferred {amount:C} from Account {from.AccountId} to Account {to.AccountId}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var accountA = new Account { AccountId = 1, AccountBalance = 1000 };
        var accountB = new Account { AccountId = 2, AccountBalance = 500 };

        var accountManager = new AccountManager();

        // Simulate concurrent transfers
        var threadA = new Thread(() => accountManager.Transfer(accountA, accountB, 200));
        var threadB = new Thread(() => accountManager.Transfer(accountB, accountA, 150));

        threadA.Start();
        threadB.Start();

        threadA.Join();
        threadB.Join();

        Console.WriteLine($"Account A balance: {accountA.AccountBalance}");
        Console.WriteLine($"Account B balance: {accountB.AccountBalance}");
    }
}