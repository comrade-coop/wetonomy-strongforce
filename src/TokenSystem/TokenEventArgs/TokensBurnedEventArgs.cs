using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
    public class TokensBurnedEventArgs<T> : EventArgs
    {
        public TokensBurnedEventArgs(decimal amount, TaggedTokens<T> tokens, Address from)
        {
            Amount = amount;
            Tokens = tokens;
            From = from;
        }

        public decimal Amount { get; }

        public TaggedTokens<T> Tokens { get; }

        public Address From { get; }
    }
}