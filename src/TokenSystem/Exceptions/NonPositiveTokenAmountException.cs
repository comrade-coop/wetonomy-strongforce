using System;
using System.Numerics;

namespace TokenSystem.Exceptions
{
    public class NonPositiveTokenAmountException : Exception
    {
        public NonPositiveTokenAmountException(BigInteger tokenAmount) : base(
            $"Non-positive token amount requested: {tokenAmount}.")
        {
        }
    }
}