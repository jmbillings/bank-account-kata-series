using System;

namespace BankingKata
{
    public class Account
    {
        private readonly ILedger _ledger;
        private readonly static Money _hardLimit = new Money(-1000);

        public Account(ILedger ledger)
        {
            _ledger = ledger;
        }

        public Account()
            : this(new Ledger())
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
            if (debitEntry.ApplyTo(CalculateBalance()) < _hardLimit)
                throw new OverdraftLimitExceededException("Overdraft limit would be exceeded");
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