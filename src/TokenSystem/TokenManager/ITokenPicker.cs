// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public interface ITokenPicker<TTagType>
	{
		IReadOnlyTaggedTokens<TTagType> Pick(IReadOnlyTaggedTokens<TTagType> tokens, BigInteger amount);
	}
}