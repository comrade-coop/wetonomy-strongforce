// Copyright (c) Comrade Coop. All rights reserved.

using ContractsCore;
using TestUtils;
using TokenSystem.Exceptions;
using TokenSystem.TokenManagerBase;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestFungibleTaggingAndPicking
	{
		private readonly FungibleTokenTagger fungibleTagger = new FungibleTokenTagger();
		private readonly FungibleTokenPicker fungiblePicker = new FungibleTokenPicker();
		private readonly Address defaultAddress = AddressTestUtils.GenerateRandomAddress();

		[Theory]
		[InlineData(100)]
		public void Tag_WhenPassedValidAddressAndAmount_TagsTokensEvenly(int amount)
		{
			IReadOnlyTaggedTokens tokens = this.fungibleTagger.Tag(this.defaultAddress, amount);
			Assert.Equal(amount, tokens.TotalBalanceByTag(FungibleTokenTagger.TokenTag));
		}

		[Theory]
		[InlineData(-100)]
		public void Tag_WhenPassedNegativeAmount_Throws(int amount)
		{
			Assert.Throws<NonPositiveTokenAmountException>(
				() => this.fungibleTagger.Tag(this.defaultAddress, amount));
		}

		[Theory]
		[InlineData(23, 3)]
		public void Pick_WhenRequestedLessThanOwnedTokens_ReturnsCorrectAmount(int mintAmount, int pickAmount)
		{
			IReadOnlyTaggedTokens tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			IReadOnlyTaggedTokens pickedTokens = this.fungiblePicker.Pick(tokens, pickAmount);
			Assert.Equal(pickAmount, pickedTokens.TotalBalanceByTag(FungibleTokenTagger.TokenTag));
		}

		[Theory]
		[InlineData(23, 100)]
		public void Pick_WhenRequestedMoreThanOwnedTokens_Throws(int mintAmount, int pickAmount)
		{
			IReadOnlyTaggedTokens tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			Assert.Throws<InsufficientTokenAmountException>(() => this.fungiblePicker.Pick(tokens, pickAmount));
		}

		[Theory]
		[InlineData(100, -10)]
		public void Pick_WhenPassedNegativeAmount_Throws(int mintAmount, int pickAmount)
		{
			IReadOnlyTaggedTokens tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			Assert.Throws<NonPositiveTokenAmountException>(() => this.fungiblePicker.Pick(tokens, pickAmount));
		}
	}
}