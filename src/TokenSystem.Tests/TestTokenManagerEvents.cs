using System.Collections.Generic;
using ContractsCore;
using TokenSystem.TokenEventArgs;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenManagerEvents
	{
		private const string Symbol = "Test";
		private const int AddressesCount = 5;

		private readonly List<Address> addresses = StrongForceHelperUtils.GenerateRandomAddresses(AddressesCount);
		private readonly TokenManager tokenManager;

		public TestTokenManagerEvents()
		{
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPickStrategy();
			this.tokenManager = new TokenManager(Symbol, tokenTagger, tokenPicker);
		}

		[Theory]
		[InlineData(1000)]
		public void ShouldRaiseTokensMintedEventCorrectly(decimal mintAmount)
		{
			Address to = this.addresses[0];
			this.tokenManager.TokensMinted += delegate(object sender, TokensMintedEventArgs args)
			{
				Assert.Equal(mintAmount, args.Amount);
				Assert.Equal(to, args.To);
			};

			this.tokenManager.Mint(mintAmount, to);
		}

		[Theory]
		[InlineData(1000, 100)]
		public void ShouldRaiseTokensTransferredEventCorrectly(decimal mintAmount, decimal transferAmount)
		{
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			this.tokenManager.TokensTransferred += delegate(object sender, TokensTransferredEventArgs args)
			{
				Assert.Equal(transferAmount, args.Amount);
				Assert.Equal(from, args.From);
				Assert.Equal(to, args.To);
			};

			this.tokenManager.Mint(mintAmount, from);
			this.tokenManager.Transfer(transferAmount, from, to);
		}

		[Theory]
		[InlineData(1000, 100)]
		public void ShouldRaiseTokensBurnedEventCorrectly(decimal mintAmount, decimal burnAmount)
		{
			Address from = this.addresses[0];

			this.tokenManager.TokensBurned += delegate(object sender, TokensBurnedEventArgs args)
			{
				Assert.Equal(burnAmount, args.Amount);
				Assert.Equal(from, args.From);
			};

			this.tokenManager.Mint(mintAmount, from);
			this.tokenManager.Burn(burnAmount, from);
		}
	}
}