using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.TokenEventArgs
{
    public class TokensBurnedEventArgs : EventArgs
    {
        public TokensBurnedEventArgs(decimal amount, IDictionary<string, decimal> tokens, Address from)
        {
            Amount = amount;
            Tokens = tokens;
            From = from;
        }

        public decimal Amount { get; }

        public IDictionary<string, decimal> Tokens { get; }

        public Address From { get; }
    }
}