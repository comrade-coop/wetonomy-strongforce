using System;

namespace TokenSystem.Exceptions
{
    public class InsufficientTokenAmountException : Exception
    {
        public InsufficientTokenAmountException(decimal total, decimal requested) :
            base($"Insufficient token amount requested: Requested {requested} from only {total}.")
        {
        }
    }
}