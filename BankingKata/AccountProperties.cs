using System;

namespace BankingKata
{
    public class AccountProperties
    {
        private Money _bankCharge;
        private OverdraftLimits _overdraftLimits;

        public AccountProperties(Money bankCharge, OverdraftLimits overdraftLimits)
        {
            _bankCharge = bankCharge;
            _overdraftLimits = overdraftLimits;
        }

        public bool ExceedsSoftLimit(Money amount)
        {
            return _overdraftLimits.ExceedsSoftLimit(amount);
        }

        public bool ExceedsHardLimit(Money amount)
        {
            return _overdraftLimits.ExceedsHardLimit(amount);
        }

        public BankChargeDebitEntry OverdraftBankCharge()
        {
            return  new BankChargeDebitEntry(DateTime.UtcNow, _bankCharge);
        }
    }
}
