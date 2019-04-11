using System;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensBurnedEventArgs<TTagType> : EventArgs
	{
		public TokensBurnedEventArgs(decimal amount, IReadOnlyTaggedTokens<TTagType> tokens, Address from)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
		}

		public decimal Amount { get; }

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address From { get; }
	}
}