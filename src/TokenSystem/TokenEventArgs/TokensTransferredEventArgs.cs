using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.TokenEventArgs
{
    public class TokensTransferredEventArgs : EventArgs
    {
        public TokensTransferredEventArgs(
            decimal amount,
            IDictionary<string, decimal> tokens, Address from,
            Address to)
        {
            Amount = amount;
            Tokens = tokens;
            From = from;
            To = to;
        }

        public decimal Amount { get; }

        public IDictionary<string, decimal> Tokens { get; }

        public Address From { get; }

        public Address To { get; }
    }
}