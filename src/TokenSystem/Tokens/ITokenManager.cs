using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public interface ITokenManager<T>
    {
        string Symbol();

        decimal BalanceOf(Address tokenHolder);

        TaggedTokens<T> TaggedBalanceOf(Address tokenHolder, object tagProps = null);

        decimal TotalBalance();

        TaggedTokens<T> TaggedTotalBalance(object tagProps = null);

        void Mint(decimal amount, Address to, object tagProps = null);

        void Transfer(decimal amount, Address from, Address to, object tagProps = null);

        void Burn(decimal amount, Address from, object tagProps = null);
    }
}