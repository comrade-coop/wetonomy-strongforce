using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;
using TokenSystem.Exceptions;
using Xunit;
using System;
using System.Linq;

namespace TokenSystem.Tests
{
    public class TestReverseDatePickerTokenTagger
    {
        private readonly DateTokenTagger dateTagger;
        private IAddressFactory addressFactory;
        private readonly Address defaultAddress;

        public TestReverseDatePickerTokenTagger()
        {
            dateTagger = new ReverseDatePickerTokenTagger();
            addressFactory = new RandomAddressFactory();
            defaultAddress = addressFactory.Create();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(25)]
        public void TagTokens_ReturnsSameAmount(decimal amount)
        {
            TaggedTokens<DateTime> tokens = dateTagger.Tag(defaultAddress, amount);
            IEnumerator<KeyValuePair<DateTime, decimal>> enumerator = tokens.GetEnumerator();
            enumerator.MoveNext();
            Assert.Equal(amount, enumerator.Current.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void TagTokens_ForInvalidNumbers_TrowsExeption(decimal amount)
        {
            Assert.Throws<NonPositiveTokenAmountException>(() => dateTagger.Tag(defaultAddress, amount));
        }

        [Theory]
        [InlineData(100000)]
        [InlineData(233)]
        public void PickTokens_ReturnsSameAmount(decimal amount)
        {
            TaggedTokens<DateTime> tokens = dateTagger.Tag(defaultAddress, amount);
            TaggedTokens<DateTime> pickedTokens = dateTagger.Pick(tokens);
            IEnumerator<KeyValuePair<DateTime, decimal>> enumerator = pickedTokens.GetEnumerator();
            enumerator.MoveNext();
            Assert.Equal(amount, enumerator.Current.Value);
        }

        [Theory]
        [InlineData(23, 3, 2.8125)]
        [InlineData(23, 0, 0)]
        public void PickTokens_ReturnsAlgorithmicTake(decimal mintAmount, decimal pickAmount, decimal expectedAmount)
        {
            TaggedTokens<DateTime> tokens = dateTagger.Tag(defaultAddress, mintAmount);
            TaggedTokens<DateTime> pickedTokens = dateTagger.Pick(tokens, pickAmount);
            IEnumerator<KeyValuePair<DateTime, decimal>> enumerator = pickedTokens.GetEnumerator();
            enumerator.MoveNext();
           
            Assert.Equal(expectedAmount, enumerator.Current.Value);
        }

        [Theory]
        [InlineData(23, 3, 0, 1.5)]
        [InlineData(23, 3, 4, 0)]
        [InlineData(23, 3, 2, 0.375)]
        [InlineData(23, 12, 2, 1.500)]
        [InlineData(23, 12, 3, 0.750)]
        [InlineData(23, 100, 3, 20.5859375)]
        [InlineData(23, 150, 3, 23)]
        public void PickTokens_ForMultipleUsers_ReturnsTakenProportion(decimal mintAmount, decimal pickAmount, int personIndex, decimal expectedAmount)
        {
            SortedDictionary<Address, TaggedTokens<DateTime>> tokens = new SortedDictionary<Address, TaggedTokens<DateTime>>();
            for (int j = 0; j < 5; j++)
            {
                Address address = addressFactory.Create();
                TaggedTokens<DateTime> tagedTokens = dateTagger.Tag(address, mintAmount, RandomDay());
                tokens.Add(address, tagedTokens);
            }
            SortedDictionary<Address, TaggedTokens<DateTime>> pickedTokens = dateTagger.Pick(tokens, pickAmount);
            //We need this, because pickedTokens are sorted per address
            var querySelectedTokens =
                from picked in pickedTokens
                from AmountPerDate in picked.Value
                orderby AmountPerDate.Key ascending
                select picked.Value.First();

            int x = pickedTokens.Count();
            if (pickedTokens.Count() < personIndex+1)
                Assert.Equal(0, expectedAmount);
            else
            {
                KeyValuePair<DateTime, decimal> realAmount = querySelectedTokens.Skip(personIndex).First();
                Assert.Equal(expectedAmount, realAmount.Value);
            }
        }

        [Theory]
        [InlineData(100, -10)]
        public void PickTokens_NegativAmount_ThrowsExeption(decimal mintAmount, decimal pickAmount)
        {
            TaggedTokens<DateTime> tokens = dateTagger.Tag(defaultAddress, mintAmount);
            Assert.Throws<NonPositiveTokenAmountException>(() => dateTagger.Pick(tokens, pickAmount));
        }

        private Random gen = new Random();
        DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}
