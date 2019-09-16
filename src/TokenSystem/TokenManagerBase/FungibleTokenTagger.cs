// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using StrongForce.Core;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public class FungibleTokenTagger : ITokenTagger
	{
		public const string TokenTag = "";

		public IDictionary<string, object> GetState()
		{
			return new Dictionary<string, object>();
		}

		public void SetState(IDictionary<string, object> state)
		{
		}

		public IReadOnlyTaggedTokens Tag(Address owner, BigInteger amount)
		{
			if (amount <= 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			var tokens = new ReadOnlyTaggedTokens(new SortedDictionary<string, BigInteger>
			{
				[TokenTag] = amount,
			});

			return tokens;
		}
	}
}