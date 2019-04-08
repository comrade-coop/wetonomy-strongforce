using System;
using System.Collections.Generic;
using ContractsCore;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenManager
	{
		private const string Symbol = "Test";
		private const int AddressesCount = 10;

		private readonly ITokenTagger tokenTagger = new FungibleTokenTagger();
		private readonly ITaggedTokenPickStrategy tokenPicker = new FungibleTokenPickStrategy();
		private readonly List<Address> addresses = StrongForceHelperUtils.GenerateRandomAddresses(AddressesCount);

		[Fact]
		public void ShouldCreateTokenManagerWithCorrectSymbol()
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Assert.Equal(Symbol, tokenManager.Symbol());
		}

		[Fact]
		public void ShouldHaveAllInitialBalancesAsZero()
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			decimal totalBalance = tokenManager.TotalBalance();
			IDictionary<string, decimal> taggedBalance = tokenManager.TaggedTotalBalance();

			Assert.Equal(0, totalBalance);
			Assert.True(taggedBalance.Keys.Count == 0);

			this.addresses.ForEach(address =>
			{
				decimal balance = tokenManager.BalanceOf(address);
				IDictionary<string, decimal> taggedBalanceOfAddress =
					tokenManager.TaggedBalanceOf(address);
				Assert.Equal(0, balance);
				Assert.True(taggedBalanceOfAddress.Keys.Count == 0);
			});
		}

		[Theory]
		[InlineData(100)]
		[InlineData(9999999999999999999)]
		public void ShouldMintTokensCorrectly(decimal amount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address receiver = this.addresses[0];

			tokenManager.Mint(amount, receiver);
			Assert.Equal(amount, tokenManager.BalanceOf(receiver));
			Assert.Equal(amount, tokenManager.TotalBalance());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-100)]
		public void ShouldThrowWhenAttemptingToMintNonPositiveTokenAmounts(decimal amount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address receiver = this.addresses[0];

			Assert.Throws<NonPositiveTokenAmountException>(() => tokenManager.Mint(amount, receiver));
		}

		[Theory]
		[InlineData(1000, 50)]
		public void ShouldTransferTokensCorrectly(decimal mintAmount, decimal transferAmount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			tokenManager.Mint(mintAmount, from);
			tokenManager.Mint(mintAmount, to);

			decimal balanceOfFromBeforeTransfer = tokenManager.BalanceOf(from);
			decimal balanceOfToBeforeTransfer = tokenManager.BalanceOf(to);

			tokenManager.Transfer(transferAmount, from, to);

			decimal balanceOfFromAfterTransfer = tokenManager.BalanceOf(from);
			decimal balanceOfToAfterTransfer = tokenManager.BalanceOf(to);

			Assert.Equal(balanceOfFromBeforeTransfer - transferAmount, balanceOfFromAfterTransfer);
			Assert.Equal(balanceOfToBeforeTransfer + transferAmount, balanceOfToAfterTransfer);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-234)]
		public void ShouldThrowWhenAttemptingToTransferNonPositiveAmounts(decimal transferAmount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			Assert.Throws<NonPositiveTokenAmountException>(() => tokenManager.Transfer(transferAmount, from, to));
		}

		[Theory]
		[InlineData(100, 5000)]
		public void ShouldThrowWhenAttemptingToTransferMoreThanOwnedTokens(decimal mintAmount, decimal transferAmount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			tokenManager.Mint(mintAmount, from);
			Assert.Throws<InsufficientTokenAmountException>(
				() => tokenManager.Transfer(transferAmount, from, to)
			);
		}

		[Fact]
		public void ShouldThrowWhenSenderAttemptingToTransferToHimself()
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			const decimal mintAmount = 100;
			const decimal transferAmount = 50;

			tokenManager.Mint(mintAmount, from);
			Assert.Throws<ArgumentException>(
				() => tokenManager.Transfer(transferAmount, from, from)
			);
		}

		[Fact]
		public void ShouldThrowWhenSenderAttemptingToTransferToNullAddress()
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			const decimal mintAmount = 100;
			const decimal transferAmount = 50;

			tokenManager.Mint(mintAmount, from);
			Assert.Throws<ArgumentNullException>(
				() => tokenManager.Transfer(transferAmount, from, null)
			);
		}

		[Theory]
		[InlineData(100, 90)]
		public void ShouldBurnTokensCorrectly(decimal mintAmount, decimal burnAmount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address address = this.addresses[0];

			tokenManager.Mint(mintAmount, address);
			decimal balanceBeforeBurn = tokenManager.BalanceOf(address);

			tokenManager.Burn(burnAmount, address);
			decimal balanceAfterBurn = tokenManager.BalanceOf(address);

			Assert.Equal(balanceBeforeBurn - burnAmount, balanceAfterBurn);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1000)]
		public void ShouldThrowWhenAttemptingToBurnNonPositiveTokenAmounts(decimal burnAmount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address address = this.addresses[0];

			Assert.Throws<NonPositiveTokenAmountException>(() => tokenManager.Burn(burnAmount, address));
		}

		[Theory]
		[InlineData(100, 110)]
		public void ShouldThrowWhenAttemptingToBurnMoreThanOwnedTokenAmount(decimal mintAmount, decimal burnAmount)
		{
			var tokenManager = new TokenManager(Symbol, this.tokenTagger, this.tokenPicker);
			Address address = this.addresses[0];

			tokenManager.Mint(mintAmount, address);

			Assert.Throws<InsufficientTokenAmountException>(() => tokenManager.Burn(burnAmount, address));
		}
	}
}