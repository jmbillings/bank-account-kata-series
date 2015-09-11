namespace BankingKata
{
    public class OverdraftLimits
    {
        private Money _hardLimit;
        private Money _softLimit;

        public OverdraftLimits(Money hardLimit, Money softLimit)
        {
            _hardLimit = hardLimit;
            _softLimit = softLimit;
        }

        public bool ExceedsSoftLimit(Money amount)
        {
            return (amount < _softLimit);
        }

        public bool ExceedsHardLimit(Money amount)
        {
            return (amount < _hardLimit);
        }
    }
}
