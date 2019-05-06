// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using TokenSystem.Exceptions;
using TokenSystem.TokenManagerBase.TokenTags;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public class FungibleTokenTagger : ITokenTagger
	{
		public const string DefaultTokenTag = "FungibleToken";

		public IReadOnlyTaggedTokens Tag(Address owner, BigInteger amount, object options = null)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			var tokens = new ReadOnlyTaggedTokens(new SortedDictionary<TokenTagBase, BigInteger>
			{
				[new StringTag(DefaultTokenTag)] = amount,
			});

			return tokens;
		}
	}
}