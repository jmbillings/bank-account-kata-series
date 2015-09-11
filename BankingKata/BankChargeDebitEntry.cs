using System;

namespace BankingKata
{
    public class BankChargeDebitEntry : DebitEntry
    {
        public BankChargeDebitEntry(DateTime transactionDate, Money transactionAmount)
            : base(transactionDate, transactionAmount)
        {
        }

        public override string ToString()
        {
            return string.Format("CHRG {0}", base.ToString());
        }
    }
}
