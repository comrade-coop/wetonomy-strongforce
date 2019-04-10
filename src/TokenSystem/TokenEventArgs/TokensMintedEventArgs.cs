using System;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensMintedEventArgs<TTagType> : EventArgs
	{
		public TokensMintedEventArgs(decimal amount, TaggedTokens<TTagType> tokens, Address to)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.To = to;
		}

		public decimal Amount { get; }

		public TaggedTokens<TTagType> Tokens { get; }

		public Address To { get; }
	}
}