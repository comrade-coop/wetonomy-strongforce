// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public interface ITokenTagger
	{
		IReadOnlyTaggedTokens Tag(Address owner, BigInteger amount);
	}
}