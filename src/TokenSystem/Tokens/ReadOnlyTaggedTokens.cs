// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class ReadOnlyTaggedTokens : IReadOnlyTaggedTokens
	{
		public ReadOnlyTaggedTokens(IDictionary<IComparable, BigInteger> tagsToBalances)
		{
			this.Copy(tagsToBalances);
		}

		public ReadOnlyTaggedTokens()
			: this(new SortedDictionary<IComparable, BigInteger>())
		{
		}

		public ReadOnlyTaggedTokens(IReadOnlyTaggedTokens tokens)
		{
			this.Copy(tokens);
		}


		public BigInteger TotalBalance { get; protected set; }

		protected IDictionary<IComparable, BigInteger> TagsToBalances { get; } =
			new SortedDictionary<IComparable, BigInteger>();

		public BigInteger TotalBalanceByTag(IComparable tag)
			=> this.TagsToBalances.ContainsKey(tag) ? this.TagsToBalances[tag] : 0;

		public IEnumerator<KeyValuePair<IComparable, BigInteger>> GetEnumerator()
		{
			return this.TagsToBalances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void Copy(IEnumerable<KeyValuePair<IComparable, BigInteger>> tagsToBalances)
		{
			foreach ((IComparable tag, BigInteger amount) in tagsToBalances)
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