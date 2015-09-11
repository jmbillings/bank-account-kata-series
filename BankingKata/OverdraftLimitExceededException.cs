using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingKata
{
    public class OverdraftLimitExceededException : Exception
    {
        public OverdraftLimitExceededException(string message) : base(message)
        {
        }
    }
}
