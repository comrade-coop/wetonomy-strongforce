using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;
using TokenSystem.Exceptions;
using TokenSystem.TokenManager;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestFungibleTokenTagger
	{
		private readonly FungibleTokenTagger fungibleTagger = new FungibleTokenTagger();
		private readonly FungibleTokenPicker fungiblePicker = new FungibleTokenPicker();
		private readonly Address defaultAddress = AddressTestHelpers.GenerateRandomAddresses(1)[0];

		[Theory]
		[InlineData(100)]
		public void Tag_WhenPassedValidAddressAndAmount_TagsTokensEvenly(BigInteger amount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, amount);
			Assert.Equal(amount, tokens.GetAmountByTag(FungibleTokenTagger.DefaultTokenTag));
		}

		[Theory]
		[InlineData(-100)]
		public void Tag_WhenPassedNegativeAmount_Throws(BigInteger amount)
		{
			Assert.Throws<NonPositiveTokenAmountException>(
				() => this.fungibleTagger.Tag(this.defaultAddress, amount));
		}

		[Theory]
		[InlineData(23, 3)]
		public void Pick_WhenRequestedLessThanOwnedTokens_ReturnsCorrectAmount(BigInteger mintAmount, BigInteger pickAmount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			IReadOnlyTaggedTokens<string> pickedTokens = this.fungiblePicker.Pick(tokens, pickAmount);
			Assert.Equal(pickAmount, pickedTokens.GetAmountByTag(FungibleTokenTagger.DefaultTokenTag));
		}

		[Theory]
		[InlineData(23, 100)]
		public void Pick_WhenRequestedMoreThanOwnedTokens_Throws(
			BigInteger mintAmount,
			BigInteger pickAmount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			Assert.Throws<InsufficientTokenAmountException>(() => this.fungiblePicker.Pick(tokens, pickAmount));
		}


		[Theory]
		[InlineData(100, -10)]
		public void Pick_WhenPassedNegativeAmount_Throws(
			BigInteger mintAmount,
			BigInteger pickAmount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			Assert.Throws<NonPositiveTokenAmountException>(() => this.fungiblePicker.Pick(tokens, pickAmount));
		}
	}
}