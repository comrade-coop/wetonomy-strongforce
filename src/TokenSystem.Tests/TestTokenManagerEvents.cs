using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.TokenEventArgs;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
    public class TestTokenManagerEvents
    {
        private const string Symbol = "Test";
        private const int AddressesCount = 5;

        private readonly List<Address> addresses;
        private readonly TokenManager tokenManager;

        public TestTokenManagerEvents()
        {
            var tokenTagger = new FungibleTokenTagger();
            tokenManager = new TokenManager(Symbol, tokenTagger);
            addresses = StrongForceHelperUtils.GenerateRandomAddresses(AddressesCount);
        }

        [Theory]
        [InlineData(1000)]
        public void ShouldRaiseTokensMintedEventCorrectly(decimal mintAmount)
        {
            Address to = addresses[0];
            tokenManager.TokensMinted += delegate(object sender, TokensMintedEventArgs args)
            {
                Assert.Equal(mintAmount, args.Amount);
                Assert.Equal(to, args.To);
            };

            tokenManager.Mint(mintAmount, to);
        }

        [Theory]
        [InlineData(1000, 100)]
        public void ShouldRaiseTokensTransferredEventCorrectly(decimal mintAmount, decimal transferAmount)
        {
            Address from = addresses[0];
            Address to = addresses[1];

            tokenManager.TokensTransferred += delegate(object sender, TokensTransferredEventArgs args)
            {
                Assert.Equal(transferAmount, args.Amount);
                Assert.Equal(from, args.From);
                Assert.Equal(to, args.To);
            };

            tokenManager.Mint(mintAmount, from);
            tokenManager.Transfer(transferAmount, from, to);
        }
        
        [Theory]
        [InlineData(1000, 100)]
        public void ShouldRaiseTokensBurnedEventCorrectly(decimal mintAmount, decimal burnAmount)
        {
            Address from = addresses[0];

            tokenManager.TokensBurned += delegate(object sender, TokensBurnedEventArgs args)
            {
                Assert.Equal(burnAmount, args.Amount);
                Assert.Equal(from, args.From);
            };

            tokenManager.Mint(mintAmount, from);
            tokenManager.Burn(burnAmount, from);
        }
    }
}