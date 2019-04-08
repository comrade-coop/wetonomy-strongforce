using System;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensMintedEventArgs : EventArgs
	{
		public TokensMintedEventArgs(decimal amount, TaggedTokens tokens, Address to)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.To = to;
		}

		public decimal Amount { get; }

		public TaggedTokens Tokens { get; }

		public Address To { get; }
	}
}