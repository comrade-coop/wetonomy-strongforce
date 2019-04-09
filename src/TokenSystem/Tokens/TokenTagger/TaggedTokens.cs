using System;
using System.Collections.Generic;
using System.Text;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
    public class TaggedTokens<T> : SortedDictionary<T, decimal>
    {
        public new void Add(T key, decimal value)
        {
            if (value < 0)
            {
                throw new NonPositiveTokenAmountException(value);
            }
            base.Add(key, value);
        }

        public void Add(KeyValuePair<T, decimal> pair)
        {
            if (pair.Value < 0)
            {
                throw new NonPositiveTokenAmountException(pair.Value);
            }
            base.Add(pair.Key, pair.Value);
        }
    }
}
