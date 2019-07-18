// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;

namespace TokenSystem.Exceptions
{
	public class NonPositiveTokenAmountException : ArgumentOutOfRangeException
	{
		public NonPositiveTokenAmountException(string paramName, BigInteger tokenAmount)
			: base(
				paramName,
				$"Non-positive token amount requested: {tokenAmount}.")
		{
		}

		public static void RequirePositiveAmount(BigInteger tokenAmount)
		{
			if (tokenAmount <= 0)
			{
				throw new NonPositiveTokenAmountException(nameof(tokenAmount), tokenAmount);
			}
		}
	}
}