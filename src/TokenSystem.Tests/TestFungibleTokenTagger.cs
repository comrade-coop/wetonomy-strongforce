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
		private readonly Address defaultAddress;

		public TestFungibleTokenTagger()
		{
			IAddressFactory addressFactory = new RandomAddressFactory();
			this.defaultAddress = addressFactory.Create();
		}

		[Theory]
		[InlineData(100)]
		public void ShouldTagTokensCorrectly(decimal amount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, amount);
			Assert.Equal(amount, tokens.GetAmountByTag(FungibleTokenTagger.DefaultTokenTag));
		}

		[Theory]
		[InlineData(-100)]
		public void ShouldThrowWhenInvalidAmountOfTokensAreTagged(decimal amount)
		{
			Assert.Throws<NonPositiveTokenAmountException>(
				() => this.fungibleTagger.Tag(this.defaultAddress, amount));
		}

		[Theory]
		[InlineData(23, 3)]
		public void ShouldPickXAmountOfTokensCorrectly(decimal mintAmount, decimal pickAmount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			IReadOnlyTaggedTokens<string> pickedTokens = this.fungiblePicker.Pick(tokens, pickAmount);
			Assert.Equal(pickAmount, pickedTokens.GetAmountByTag(FungibleTokenTagger.DefaultTokenTag));
		}

		[Theory]
		[InlineData(100, -10)]
		public void ShouldThrowWhenPickingNonPositiveAmountOfTokens(decimal mintAmount, decimal pickAmount)
		{
			IReadOnlyTaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			Assert.Throws<NonPositiveTokenAmountException>(() => this.fungiblePicker.Pick(tokens, pickAmount));
		}
	}
}