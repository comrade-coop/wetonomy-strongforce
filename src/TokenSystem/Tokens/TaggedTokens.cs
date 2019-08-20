// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class TaggedTokens : ReadOnlyTaggedTokens, ITaggedTokens
	{
		public TaggedTokens()
			: base(new SortedDictionary<string, BigInteger>())
		{
		}

		public TaggedTokens(IReadOnlyTaggedTokens tokens)
			: base(tokens)
		{
		}

		public TaggedTokens(IDictionary<string, object> state)
			: base(state)
		{
		}

		public void AddToBalance(string tag, BigInteger amount)
		{
			NonPositiveTokenAmountException.RequirePositiveAmount(amount);
			if (!this.TagsToBalances.ContainsKey(tag))
			{
				this.TagsToBalances[tag] = 0;
			}

			this.TagsToBalances[tag] += amount;
			this.TotalBalance += amount;
		}

		public void AddToBalance(IReadOnlyTaggedTokens tokens)
		{
			foreach ((string tag, BigInteger amount) in tokens)
			{
				this.AddToBalance(tag, amount);
			}
		}

		public void RemoveFromBalance(string tag, BigInteger amount)
		{
			NonPositiveTokenAmountException.RequirePositiveAmount(amount);
			if (!this.TagsToBalances.ContainsKey(tag))
			{
				this.TagsToBalances[tag] = 0;
			}

			this.TagsToBalances[tag] -= amount;
			this.TotalBalance -= amount;
		}

		public void RemoveFromBalance(IReadOnlyTaggedTokens tokens)
		{
			foreach ((string tag, BigInteger amount) in tokens)
			{
				this.RemoveFromBalance(tag, amount);
			}
		}
	}
}