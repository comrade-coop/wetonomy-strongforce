using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;
using TokenSystem.Exceptions;
using Xunit;

namespace TokenSystem.Tests
{
    public class TestFungibleTokenTagger
    {
        private readonly ITokenTagger<string> fungibleTagger;
        private readonly Address defaultAddress;

        public TestFungibleTokenTagger()
        {
            this.fungibleTagger = new FungibleTokenTagger();
            IAddressFactory addressFactory = new RandomAddressFactory();
            this.defaultAddress = addressFactory.Create();
        }

        [Theory]
        [InlineData(100)]
        public void ShouldTagTokensCorrectly(decimal amount)
        {
            TaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, amount);
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
        [InlineData(100000)]
        public void ShouldPickAllTokensCorrectly(decimal amount)
        {
            TaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, amount);
            TaggedTokens<string> pickedTokens = this.fungibleTagger.Pick(tokens);
            Assert.Equal(amount, pickedTokens[FungibleTokenTagger.DefaultTokenTag]);
        }

        [Theory]
        [InlineData(23, 3)]
        [InlineData(23, 0)]
        public void ShouldPickXAmountOfTokensCorrectly(decimal mintAmount, decimal pickAmount)
        {
            TaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
            TaggedTokens<string> pickedTokens = this.fungibleTagger.Pick(tokens, pickAmount);
            Assert.Equal(pickAmount, pickedTokens[FungibleTokenTagger.DefaultTokenTag]);
        }

        [Theory]
        [InlineData(100, -10)]
        public void ShouldThrowWhenPickingNegativeAmountOfTokens(decimal mintAmount, decimal pickAmount)
        {
            TaggedTokens<string> tokens = this.fungibleTagger.Tag(this.defaultAddress, mintAmount);
            Assert.Throws<NonPositiveTokenAmountException>(() => this.fungibleTagger.Pick(tokens, pickAmount));
        }
    }
}