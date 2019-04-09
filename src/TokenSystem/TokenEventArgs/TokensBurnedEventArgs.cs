using System;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensBurnedEventArgs<TTagType> : EventArgs
	{
		public TokensBurnedEventArgs(decimal amount, TaggedTokens<TTagType> tokens, Address from)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
		}

		public decimal Amount { get; }

		public TaggedTokens<TTagType> Tokens { get; }

		public Address From { get; }
	}
}