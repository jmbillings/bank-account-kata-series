using System;

namespace BankingKata
{
    public class Account
    {
        private readonly ILedger _ledger;
        private readonly AccountProperties _accountProperties;

        public Account(ILedger ledger, Money hardLimit, Money softLimit, Money bankCharge)
        {
            _ledger = ledger;
            _accountProperties = new AccountProperties(bankCharge, new OverdraftLimits(hardLimit, softLimit));

            if (hardLimit > softLimit)
                throw new ArgumentOutOfRangeException("Account Hard limit must be below the Soft limit");
        }

        public Account()
            : this(new Ledger(), new Money(-1000), new Money(-200), new Money(15))
        {
        }

        public void Deposit(DateTime transactionDate, Money money)
        {
            var depositTransaction = new CreditEntry(transactionDate, money);
            _ledger.Record(depositTransaction);
        }

        public Money CalculateBalance()
        {
            return _ledger.Accept(new BalanceCalculatingVisitor(), new Money(0m));
        }

        public void Withdraw(DebitEntry debitEntry)
        {
            if (_accountProperties.ExceedsHardLimit(debitEntry.ApplyTo(CalculateBalance())))
                throw new OverdraftLimitExceededException("Overdraft limit would be exceeded");

            if (_accountProperties.ExceedsSoftLimit(debitEntry.ApplyTo(CalculateBalance())))
            {
                DebitEntry bankCharge = _accountProperties.OverdraftBankCharge();
                _ledger.Record(bankCharge);
            }

            _ledger.Record(debitEntry);
        }

        public void PrintBalance(IPrinter printer)
        {
            var balance = CalculateBalance();
            printer.PrintBalance(balance);
        }

        public void PrintLastTransaction(IPrinter printer)
        {
            printer.PrintLastTransaction(_ledger);
        }
    }
}