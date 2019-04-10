using System;
using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenManager
	{
		private const int AddressesCount = 10;

		private readonly ITokenTagger<string> tokenTagger = new FungibleTokenTagger();
		private readonly ITokenPicker<string> tokenPicker = new FungibleTokenPicker();
		private readonly List<Address> addresses = AddressHelpers.GenerateRandomAddresses(AddressesCount);

		[Fact]
		public void ShouldHaveAllInitialBalancesAsZero()
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			TaggedTokens<string> taggedBalance = tokenManager.TaggedTotalBalance();

			Assert.Equal(0, taggedBalance.TotalTokens);

			this.addresses.ForEach(address =>
			{
				TaggedTokens<string> balance = tokenManager.TaggedBalanceOf(address);
				Assert.Equal(0, balance.TotalTokens);
			});
		}

		[Theory]
		[InlineData(100)]
		[InlineData(9999999999999999999)]
		public void ShouldMintTokensCorrectly(decimal amount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address receiver = this.addresses[0];

			tokenManager.Mint(amount, receiver);
			Assert.Equal(amount, tokenManager.TaggedBalanceOf(receiver).TotalTokens);
			Assert.Equal(amount, tokenManager.TaggedTotalBalance().TotalTokens);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-100)]
		public void ShouldThrowWhenAttemptingToMintNonPositiveTokenAmounts(decimal amount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address receiver = this.addresses[0];

			Assert.Throws<NonPositiveTokenAmountException>(() => tokenManager.Mint(amount, receiver));
		}

		[Theory]
		[InlineData(1000, 50)]
		public void ShouldTransferTokensCorrectly(decimal mintAmount, decimal transferAmount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			tokenManager.Mint(mintAmount, from);
			tokenManager.Mint(mintAmount, to);

			decimal balanceFromBeforeTransfer = tokenManager.TaggedBalanceOf(from).TotalTokens;
			decimal balanceToBeforeTransfer = tokenManager.TaggedBalanceOf(to).TotalTokens;

			tokenManager.Transfer(transferAmount, from, to);

			TaggedTokens<string> balanceFromAfterTransfer = tokenManager.TaggedBalanceOf(from);
			TaggedTokens<string> balanceOfToAfterTransfer = tokenManager.TaggedBalanceOf(to);

			Assert.Equal(balanceFromBeforeTransfer - transferAmount,
				balanceFromAfterTransfer.TotalTokens);
			Assert.Equal(balanceToBeforeTransfer + transferAmount,
				balanceOfToAfterTransfer.TotalTokens);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-234)]
		public void ShouldThrowWhenAttemptingToTransferNonPositiveAmounts(decimal transferAmount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			Assert.Throws<NonPositiveTokenAmountException>(
				() => tokenManager.Transfer(transferAmount, from, to));
		}

		[Theory]
		[InlineData(100, 5000)]
		public void ShouldThrowWhenAttemptingToTransferMoreThanOwnedTokens(decimal mintAmount, decimal transferAmount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
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
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address from = this.addresses[0];
			const decimal mintAmount = 100;
			const decimal transferAmount = 50;

			tokenManager.Mint(mintAmount, from);
			Assert.Throws<ArgumentException>(
				() => tokenManager.Transfer(transferAmount, from, from)
			);
		}

		[Theory]
		[InlineData(100, 90)]
		public void ShouldBurnTokensCorrectly(decimal mintAmount, decimal burnAmount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address address = this.addresses[0];

			tokenManager.Mint(mintAmount, address);
			decimal balanceBeforeBurn = tokenManager.TaggedBalanceOf(address).TotalTokens;

			tokenManager.Burn(burnAmount, address);
			TaggedTokens<string> balanceAfterBurn = tokenManager.TaggedBalanceOf(address);

			Assert.Equal(balanceBeforeBurn - burnAmount, balanceAfterBurn.TotalTokens);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1000)]
		public void ShouldThrowWhenAttemptingToBurnNonPositiveTokenAmounts(decimal burnAmount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address address = this.addresses[0];
			Assert.Throws<NonPositiveTokenAmountException>(() => tokenManager.Burn(burnAmount, address));
		}

		[Theory]
		[InlineData(100, 110)]
		public void ShouldThrowWhenAttemptingToBurnMoreThanOwnedTokenAmount(decimal mintAmount, decimal burnAmount)
		{
			var tokenManager = new TokenManager<string>(this.tokenTagger, this.tokenPicker);
			Address address = this.addresses[0];

			tokenManager.Mint(mintAmount, address);

			Assert.Throws<InsufficientTokenAmountException>(() => tokenManager.Burn(burnAmount, address));
		}
	}
}