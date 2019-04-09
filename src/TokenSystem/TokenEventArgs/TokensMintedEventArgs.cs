using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
    public class TokensMintedEventArgs<T> : EventArgs
    {
        public TokensMintedEventArgs(decimal amount, TaggedTokens<T> tokens, Address to)
        {
            Amount = amount;
            Tokens = tokens;
            To = to;
        }

        public decimal Amount { get; }

        public TaggedTokens<T> Tokens { get; }

        public Address To { get; }
    }
}