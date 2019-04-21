// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using ContractsCore;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public class FungibleTokenTagger : ITokenTagger<string>
	{
		public const string DefaultTokenTag = "FungibleToken";

		public IReadOnlyTaggedTokens<string> Tag(Address owner, decimal amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			var tokens = new ReadOnlyTaggedTokens<string>(new SortedDictionary<string, decimal>
			{
				[DefaultTokenTag] = amount,
			});

			return tokens;
		}
	}
}