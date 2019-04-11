using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenManagerEvents
	{
		private const int AddressesCount = 5;

		private readonly List<Address> addresses;
		private readonly TokenManager<string> tokenManager;

		public TestTokenManagerEvents()
		{
			var tokenTagger = new FungibleTokenTagger();
			this.tokenManager = new TokenManager<string>(tokenTagger, new FungibleTokenPicker());
			this.addresses = AddressHelpers.GenerateRandomAddresses(AddressesCount);
		}

		[Theory]
		[InlineData(1000)]
		public void ShouldRaiseTokensMintedEventCorrectly(decimal mintAmount)
		{
			Address to = this.addresses[0];
			this.tokenManager.TokensMinted += delegate(object sender, TokensMintedEventArgs<string> args)
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

			this.tokenManager.TokensTransferred += delegate(object sender, TokensTransferredEventArgs<string> args)
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

			this.tokenManager.TokensBurned += delegate(object sender, TokensBurnedEventArgs<string> args)
			{
				Assert.Equal(burnAmount, args.Amount);
				Assert.Equal(from, args.From);
			};

			this.tokenManager.Mint(mintAmount, from);
			this.tokenManager.Burn(burnAmount, from);
		}
	}
}