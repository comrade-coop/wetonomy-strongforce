using System;
using System.Numerics;

namespace TokenSystem.Exceptions
{
    public class InsufficientTokenAmountException : Exception
    {
        public InsufficientTokenAmountException(BigInteger total, BigInteger requested) :
            base($"Insufficient token amount requested: Requested {requested} from only {total}.")
        {
        }
    }
}