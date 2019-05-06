// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public interface ITokenPicker
	{
		IReadOnlyTaggedTokens Pick(IReadOnlyTaggedTokens tokens, BigInteger amount, object options = null);
	}
}