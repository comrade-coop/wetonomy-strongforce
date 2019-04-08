using System;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensTransferredEventArgs : EventArgs
	{
		public TokensTransferredEventArgs(
			decimal amount,
			TaggedTokens tokens, Address from,
			Address to)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
			this.To = to;
		}

		public decimal Amount { get; }

		public TaggedTokens Tokens { get; }

		public Address From { get; }

		public Address To { get; }
	}
}