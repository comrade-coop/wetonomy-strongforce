using System;

namespace TokenSystem.Exceptions
{
    public class NonPositiveTokenAmountException : Exception
    {
        public NonPositiveTokenAmountException(decimal tokenAmount) : base(
            $"Non-positive token amount requested: {tokenAmount}.")
        {
        }
    }
}