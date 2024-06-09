


using System;
using System.Threading;

class Account
{
    public int AccountId { get; set; }
    public double Balance { get; set; }

    private readonly object balanceLock = new object();

    public bool Withdraw(double amount)
    {
        lock (balanceLock)
        {
            if (amount > Balance)
            {
                return false;
            }
            Balance -= amount;
            return true;
        }
    }

    public void Deposit(double amount)
    {
        lock (balanceLock)
        {
            Balance += amount;
        }
    }

    public override string ToString()
    {
        return $"Account ID: {AccountId}, Balance: {Balance}";
    }
}

class AccountManager
{
    public Account FromAccount { get; set; }
    public Account ToAccount { get; set; }
    public double TransferAmount { get; set; }

    public void Transfer()
    {
        if (FromAccount.Withdraw(TransferAmount))
        {
            ToAccount.Deposit(TransferAmount);
            Console.WriteLine($"Transferred {TransferAmount} from account {FromAccount.AccountId} to account {ToAccount.AccountId}");
        }
        else
        {
            Console.WriteLine($"Insufficient funds in account {FromAccount.AccountId}");
        }
    }

    static void TransferMoney(AccountManager accountManager)
    {
        accountManager.Transfer();
    }

    static void Main(string[] args)
    {
        Console.Write("Enter Account A ID:");
        int accountIdA = int.Parse(Console.ReadLine());

        Console.Write("Enter Account B ID:");
        int accountIdB = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter Transfer Amount:");
        double transferAmount = double.Parse(Console.ReadLine());

        Account accountA = new Account { AccountId = accountIdA, Balance = 10000 }; // Set initial balance
        Account accountB = new Account { AccountId = accountIdB, Balance = 3000 }; // Set initial balance

        AccountManager accountManagerAtoB = new AccountManager { FromAccount = accountA, ToAccount = accountB, TransferAmount = transferAmount };
        AccountManager accountManagerBtoA = new AccountManager { FromAccount = accountB, ToAccount = accountA, TransferAmount = transferAmount };

        Thread transferThreadAtoB = new Thread(() => TransferMoney(accountManagerAtoB));
        Thread transferThreadBtoA = new Thread(() => TransferMoney(accountManagerBtoA));

        transferThreadAtoB.Start();
        transferThreadBtoA.Start();

        transferThreadAtoB.Join();
        transferThreadBtoA.Join();

        Console.WriteLine("Accounts after transfer:");
        Console.WriteLine(accountA);
        Console.WriteLine(accountB);
    }
}
