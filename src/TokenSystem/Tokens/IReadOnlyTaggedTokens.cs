// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface IReadOnlyTaggedTokens<TTagType> : IEnumerable<KeyValuePair<TTagType, BigInteger>>
	{
		BigInteger TotalTokens { get; }

		BigInteger GetAmountByTag(TTagType tag);
	}
}