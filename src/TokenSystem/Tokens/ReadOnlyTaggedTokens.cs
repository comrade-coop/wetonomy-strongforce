// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class ReadOnlyTaggedTokens<TTagType> : IReadOnlyTaggedTokens<TTagType>
	{
		public ReadOnlyTaggedTokens(IDictionary<TTagType, BigInteger> initialTagsToBalances)
		{
			foreach ((TTagType _, BigInteger amount) in initialTagsToBalances)
			{
				if (amount < 0)
				{
					throw new NonPositiveTokenAmountException(amount);
				}

				this.TotalTokens += amount;
			}

			this.TagsToBalances = initialTagsToBalances;
		}

		public ReadOnlyTaggedTokens()
			: this(new SortedDictionary<TTagType, BigInteger>())
		{
		}

		public BigInteger TotalTokens { get; protected set; }

		protected IDictionary<TTagType, BigInteger> TagsToBalances { get; set; }

		public BigInteger GetAmountByTag(TTagType tag)
			=> this.TagsToBalances.ContainsKey(tag) ? this.TagsToBalances[tag] : 0;

		public IEnumerator<KeyValuePair<TTagType, BigInteger>> GetEnumerator()
		{
			return this.TagsToBalances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}