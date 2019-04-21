// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public static class TokensUtility
	{
		public static void RequirePositiveAmount(BigInteger tokenAmount)
		{
			if (tokenAmount <= 0)
			{
				throw new NonPositiveTokenAmountException(tokenAmount);
			}
		}
	}
}