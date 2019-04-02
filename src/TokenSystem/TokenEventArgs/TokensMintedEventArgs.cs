using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.TokenEventArgs
{
    public class TokensMintedEventArgs : EventArgs
    {
        public TokensMintedEventArgs(decimal amount, IDictionary<string, decimal> tokens, Address to)
        {
            Amount = amount;
            Tokens = tokens;
            To = to;
        }

        public decimal Amount { get; }

        public IDictionary<string, decimal> Tokens { get; }

        public Address To { get; }
    }
}