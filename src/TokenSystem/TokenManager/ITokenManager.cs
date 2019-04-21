// Copyright (c) Comrade Coop. All rights reserved.

using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public interface ITokenManager<TTagType>
	{
		IReadOnlyTaggedTokens<TTagType> TaggedBalanceOf(Address tokenHolder);

		IReadOnlyTaggedTokens<TTagType> TaggedTotalBalance();
	}
}