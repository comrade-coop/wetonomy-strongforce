using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
    public class TokensTransferredEventArgs<T> : EventArgs
    {
        public TokensTransferredEventArgs(
            decimal amount,
            TaggedTokens<T> tokens,
            Address from,
            Address to)
        {
            this.Amount = amount;
            this.Tokens = tokens;
            this.From = from;
            this.To = to;
        }

        public decimal Amount { get; }

        public TaggedTokens<T> Tokens { get; }

        public Address From { get; }

        public Address To { get; }
    }
}