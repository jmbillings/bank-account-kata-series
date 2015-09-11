using System;

namespace BankingKata
{
    public class Account
    {
        private readonly ILedger _ledger;
        private Money _hardLimit = new Money(-1000);
        private Money _softLimit = new Money(-200);
        private Money _bankCharge = new Money(15m);

        public Account(ILedger ledger, Money hardLimit, Money softLimit, Money bankCharge)
        {
            _ledger = ledger;
            _hardLimit = hardLimit;
            _softLimit = softLimit;
            _bankCharge = bankCharge;

            if (_hardLimit > _softLimit)
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
            if (debitEntry.ApplyTo(CalculateBalance()) < _hardLimit)
                throw new OverdraftLimitExceededException("Overdraft limit would be exceeded");

            if (debitEntry.ApplyTo(CalculateBalance()) < _softLimit)
            {
                DebitEntry bankCharge = new BankChargeDebitEntry(DateTime.UtcNow, _bankCharge);
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