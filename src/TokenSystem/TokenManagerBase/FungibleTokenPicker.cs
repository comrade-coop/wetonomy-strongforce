// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using TokenSystem.Exceptions;
using TokenSystem.TokenManagerBase.TokenTags;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public class FungibleTokenPicker : ITokenPicker
	{
		public IReadOnlyTaggedTokens Pick(IReadOnlyTaggedTokens tokens, BigInteger amount, object options = null)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			BigInteger availableTokens = tokens.GetAmountByTag(new StringTag(FungibleTokenTagger.DefaultTokenTag));

			if (availableTokens < amount)
			{
				throw new InsufficientTokenAmountException(availableTokens, amount);
			}

			var pickedTokens = new ReadOnlyTaggedTokens(new SortedDictionary<TokenTagBase, BigInteger>
			{
				[new StringTag(FungibleTokenTagger.DefaultTokenTag)] = amount,
			});

			return pickedTokens;
		}
	}
}