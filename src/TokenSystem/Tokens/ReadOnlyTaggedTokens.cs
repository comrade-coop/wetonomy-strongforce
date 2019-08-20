// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class ReadOnlyTaggedTokens : IReadOnlyTaggedTokens
	{
		public ReadOnlyTaggedTokens(IDictionary<string, BigInteger> tagsToBalances)
		{
			this.Copy(tagsToBalances);
		}

		public ReadOnlyTaggedTokens()
			: this(new SortedDictionary<string, BigInteger>())
		{
		}

		public ReadOnlyTaggedTokens(IReadOnlyTaggedTokens tokens)
		{
			this.Copy(tokens);
		}

		public ReadOnlyTaggedTokens(IDictionary<string, object> state)
		{
			this.Copy(state.Select(kv =>
			{
				return KeyValuePair.Create(
					kv.Key,
					BigInteger.Parse((string)kv.Value));
			}));
		}

		public BigInteger TotalBalance { get; protected set; }

		protected IDictionary<string, BigInteger> TagsToBalances { get; } =
			new SortedDictionary<string, BigInteger>();

		public BigInteger TotalBalanceByTag(string tag)
			=> this.TagsToBalances.ContainsKey(tag) ? this.TagsToBalances[tag] : 0;

		public IDictionary<string, object> GetState()
		{
			return this.TagsToBalances.ToDictionary(
				kv => kv.Key.ToString(),
				kv => (object)kv.Value.ToString());
		}

		public IEnumerator<KeyValuePair<string, BigInteger>> GetEnumerator()
		{
			return this.TagsToBalances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void Copy(IEnumerable<KeyValuePair<string, BigInteger>> tagsToBalances)
		{
			foreach ((string tag, BigInteger amount) in tagsToBalances)
			{
				if (amount < 0)
				{
					throw new NonPositiveTokenAmountException(nameof(amount), amount);
				}

				this.TotalBalance += amount;
				this.TagsToBalances[tag] = amount;
			}
		}
	}
}