using System;
using System.Collections.Generic;
using System.Text;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public abstract class DateTokenTagger : ITokenTagger<DateTime>
    {
       
        

        public TaggedTokens<DateTime> Tag(Address owner, decimal amount, object tagProps = null)
        {
            if (amount <= 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }

            DateTime currentDate;
            if (tagProps == null)
                currentDate = DateTime.Now;
            else currentDate = (DateTime) tagProps;

            return new TaggedTokens<DateTime> { [currentDate] = amount };
        }

        public abstract TaggedTokens<DateTime> Pick(TaggedTokens<DateTime> tokens, decimal amount, object tagProps = null);

        public abstract TaggedTokens<DateTime> Pick(TaggedTokens<DateTime> tokens, object tagProps = null);

        public abstract SortedDictionary<Address, TaggedTokens<DateTime>> Pick(SortedDictionary<Address, TaggedTokens<DateTime>> tokens,
            decimal amount, object tagProps = null);


    }
}
