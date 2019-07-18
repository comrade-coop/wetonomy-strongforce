// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface IReadOnlyTaggedTokens : IEnumerable<KeyValuePair<IComparable, BigInteger>>
	{
		BigInteger TotalBalance { get; }

		BigInteger TotalBalanceByTag(IComparable tag);
	}
}