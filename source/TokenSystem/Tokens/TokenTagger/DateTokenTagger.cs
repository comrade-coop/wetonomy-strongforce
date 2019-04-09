using System;
using System.Collections.Generic;
using System.Text;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    abstract class DateTokenTagger : ITokenTagger
    {
        public IDictionary<string, decimal> Tag(Address owner, decimal amount, ITagProperties tagProperties = null)
        {
            if (amount <= 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }
            DateTime currentDate = new DateTime();
            return new SortedDictionary<string, decimal> { [currentDate.ToString()] = amount };
        }

        public abstract IDictionary<string, decimal> Pick(IDictionary<string, decimal> tokens, decimal amount, ITagProperties tagProperties = null);

        public abstract IDictionary<string, decimal> Pick(IDictionary<string, decimal> tokens, ITagProperties tagProperties = null);
    }
}
