// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public class FungibleTokenPicker : ITokenPicker
	{
		public const string TokenTag = FungibleTokenTagger.TokenTag;

		public IDictionary<string, object> GetState()
		{
			return new Dictionary<string, object>();
		}

		public void SetState(IDictionary<string, object> state)
		{
		}

		public IReadOnlyTaggedTokens Pick(IReadOnlyTaggedTokens tokens, BigInteger amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			BigInteger availableTokens = tokens.TotalBalanceByTag(TokenTag);

			if (availableTokens < amount)
			{
				throw new InsufficientTokenAmountException(nameof(amount), availableTokens, amount);
			}

			var pickedTokens = new ReadOnlyTaggedTokens(new SortedDictionary<string, BigInteger>
			{
				[TokenTag] = amount,
			});

			return pickedTokens;
		}
	}
}