using System.Collections.Generic;
using ContractsCore;
using TokenSystem.Tokens;
using TokenSystem.Exceptions;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestFungibleTokenTagger
	{
		private readonly ITokenTagger fungibleTagger = new FungibleTokenTagger();
		private readonly ITaggedTokenPickStrategy fungiblePicker = new FungibleTokenPickStrategy();
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
			IDictionary<string, decimal> tokens = this.fungibleTagger.Tag(this.defaultAddress, amount);
			Assert.Equal(amount, tokens[FungibleTokenTagger.DefaultTokenTag]);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-100)]
		public void ShouldThrowWhenInvalidAmountOfTokensAreTagged(decimal amount)
		{
			Assert.Throws<NonPositiveTokenAmountException>(
				() => this.fungibleTagger.Tag(this.defaultAddress, amount));
		}

		[Theory]
		[InlineData(23, 3)]
		[InlineData(23, 0)]
		public void ShouldPickXAmountOfTokensCorrectly(decimal mintAmount, decimal pickAmount)
		{
			TaggedTokens tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			IDictionary<string, decimal> pickedTokens = this.fungiblePicker.Pick(tokens, pickAmount);
			Assert.Equal(pickAmount, pickedTokens[FungibleTokenTagger.DefaultTokenTag]);
		}

		[Theory]
		[InlineData(100, -10)]
		public void ShouldThrowWhenPickingNegativeAmountOfTokens(decimal mintAmount, decimal pickAmount)
		{
			TaggedTokens tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
			Assert.Throws<NonPositiveTokenAmountException>(() => this.fungiblePicker.Pick(tokens, pickAmount));
		}
	}
}