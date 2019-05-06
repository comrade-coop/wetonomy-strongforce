// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface IReadOnlyTaggedTokens: IEnumerable<KeyValuePair<TokenTagBase, BigInteger>>
	{
		BigInteger TotalTokens { get; }

		BigInteger GetAmountByTag(TokenTagBase tag);
	}
}