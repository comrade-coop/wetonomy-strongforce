// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public class FungibleTokenTagger : ITokenTagger
	{
		public const bool TokenTag = false;

		public IReadOnlyTaggedTokens Tag(Address owner, BigInteger amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			var tokens = new ReadOnlyTaggedTokens(new SortedDictionary<IComparable, BigInteger>
			{
				[TokenTag] = amount,
			});

			return tokens;
		}
	}
}