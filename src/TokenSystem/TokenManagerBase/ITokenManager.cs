// Copyright (c) Comrade Coop. All rights reserved.

using StrongForce.Core;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public interface ITokenManager
	{
		IReadOnlyTaggedTokens TaggedBalanceOf(Address tokenHolder);

		IReadOnlyTaggedTokens TaggedTotalBalance();
	}
}