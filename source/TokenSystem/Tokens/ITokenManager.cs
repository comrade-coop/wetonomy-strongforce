using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public interface ITokenManager
    {
        string Symbol();

        decimal BalanceOf(Address tokenHolder);

        IDictionary<string, decimal> TaggedBalanceOf(Address tokenHolder, ITagProperties tagProperties = null);

        decimal TotalBalance();

        IDictionary<string, decimal> TaggedTotalBalance(ITagProperties tagProperties = null);

        void Mint(decimal amount, Address to, ITagProperties tagProperties = null);

        void Transfer(decimal amount, Address from, Address to, ITagProperties tagProperties = null);

        void Burn(decimal amount, Address from, ITagProperties tagProperties = null);
    }
}