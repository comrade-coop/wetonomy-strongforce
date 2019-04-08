using System;
using System.Collections.Generic;
using ContractsCore;

namespace TokenSystem.TokenEventArgs
{
	public class TokensBurnedEventArgs : EventArgs
	{
		public TokensBurnedEventArgs(decimal amount, IDictionary<string, decimal> tokens, Address from)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
		}

		public decimal Amount { get; }

		public IDictionary<string, decimal> Tokens { get; }

		public Address From { get; }
	}
}