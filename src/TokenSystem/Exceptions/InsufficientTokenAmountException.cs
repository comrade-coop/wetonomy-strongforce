// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;

namespace TokenSystem.Exceptions
{
	public class InsufficientTokenAmountException : ArgumentOutOfRangeException
	{
		public InsufficientTokenAmountException(string paramName, BigInteger total, BigInteger requested)
			: base(
				paramName,
				$"Insufficient token amount requested: Requested {requested} from only {total}.")
		{
		}
	}
}