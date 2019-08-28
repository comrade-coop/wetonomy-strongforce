// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using StrongForce.Core;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public interface ITokenTagger : IStatefulObject
	{
		IReadOnlyTaggedTokens Tag(Address owner, BigInteger amount);
	}
}