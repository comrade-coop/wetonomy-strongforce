using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;
using TokenSystem.Exceptions;
using Xunit;

namespace TokenSystem.Tests
{
    public class TestFungibleTokenTagger
    {
        private readonly ITokenTagger fungibleTagger;
        private readonly Address defaultAddress;

        public TestFungibleTokenTagger()
        {
            fungibleTagger = new FungibleTokenTagger();
            IAddressFactory addressFactory = new RandomAddressFactory();
            defaultAddress = addressFactory.Create();
        }

        [Theory]
        [InlineData(100)]
        public void ShouldTagTokensCorrectly(decimal amount)
        {
            IDictionary<string, decimal> tokens = fungibleTagger.Tag(defaultAddress, amount);
            Assert.Equal(amount, tokens[FungibleTokenTagger.DefaultTokenTag]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void ShouldThrowWhenInvalidAmountOfTokensAreTagged(decimal amount)
        {
            Assert.Throws<NonPositiveTokenAmountException>(() => fungibleTagger.Tag(defaultAddress, amount));
        }

        [Theory]
        [InlineData(100000)]
        public void ShouldPickAllTokensCorrectly(decimal amount)
        {
            IDictionary<string, decimal> tokens = fungibleTagger.Tag(defaultAddress, amount);
            IDictionary<string, decimal> pickedTokens = fungibleTagger.Pick(tokens);
            Assert.Equal(amount, pickedTokens[FungibleTokenTagger.DefaultTokenTag]);
        }

        [Theory]
        [InlineData(23, 3)]
        [InlineData(23, 0)]
        public void ShouldPickXAmountOfTokensCorrectly(decimal mintAmount, decimal pickAmount)
        {
            IDictionary<string, decimal> tokens = fungibleTagger.Tag(defaultAddress, mintAmount);
            IDictionary<string, decimal> pickedTokens = fungibleTagger.Pick(tokens, pickAmount);
            Assert.Equal(pickAmount, pickedTokens[FungibleTokenTagger.DefaultTokenTag]);
        }

        [Theory]
        [InlineData(100, -10)]
        public void ShouldThrowWhenPickingNegativeAmountOfTokens(decimal mintAmount, decimal pickAmount)
        {
            IDictionary<string, decimal> tokens = fungibleTagger.Tag(defaultAddress, mintAmount);
            Assert.Throws<NonPositiveTokenAmountException>(() => fungibleTagger.Pick(tokens, pickAmount));
        }
    }
}