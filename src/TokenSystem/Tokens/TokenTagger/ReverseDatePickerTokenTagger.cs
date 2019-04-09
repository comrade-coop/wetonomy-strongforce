using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public class ReverseDatePickerTokenTagger : DateTokenTagger
    {
        public const decimal Weight = 0.5M;
        public const decimal DevideLimit = 0.25M;
        public override TaggedTokens<DateTime> Pick(TaggedTokens<DateTime> tokens, decimal amount, object tagProps = null)
        {
            if (amount < 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }

            TaggedTokens<DateTime> pickedTokens = new TaggedTokens<DateTime>();
            IEnumerator<KeyValuePair<DateTime, decimal>> enumerator = tokens.GetEnumerator();

            decimal lastIterAmount = amount;
            while (amount > DevideLimit)
            {
                if (!enumerator.MoveNext())
                {
                    enumerator.Reset();
                    enumerator.MoveNext();
                    if (lastIterAmount == amount) break;
                    else lastIterAmount = amount;
                }

                pickedTokens.TryGetValue(enumerator.Current.Key, out decimal x);
                if (enumerator.Current.Value - x > 0)
                {
                    if (enumerator.Current.Value > amount * Weight)
                        UpdatePickedTokens(ref pickedTokens, ref amount, amount * Weight, enumerator.Current);

                    else UpdatePickedTokens(ref pickedTokens, ref amount, enumerator.Current.Value, enumerator.Current);
                }

                
            }
            
            return pickedTokens;
        }

        private void UpdatePickedTokens(ref TaggedTokens<DateTime> pickedTokens, ref decimal amount, decimal realAmount , KeyValuePair<DateTime, decimal> current)
        {
            if (pickedTokens.ContainsKey(current.Key))
                pickedTokens[current.Key] += realAmount;

            else pickedTokens.Add(current.Key, realAmount);

            amount -= realAmount;
        }

        public override TaggedTokens<DateTime> Pick(TaggedTokens<DateTime> tokens, object tagProps = null)
            => tokens;

        public override SortedDictionary<Address, TaggedTokens<DateTime>> Pick(SortedDictionary<Address, TaggedTokens<DateTime>> tokens, decimal amount, object tagProps = null)
        {
            TaggedTokens<DateTime> sortedTokens = new TaggedTokens<DateTime>();
            foreach (var tokensPerAddress in tokens)
            {
                foreach (var t in tokensPerAddress.Value)
                {
                    sortedTokens.Add(t.Key, t.Value);
                }
            }

            TaggedTokens<DateTime> pickedTokens = Pick(sortedTokens, amount);
            SortedDictionary<Address, TaggedTokens<DateTime>> sortedTokensPerAddress = new SortedDictionary<Address, TaggedTokens<DateTime>>();

            foreach (var current in pickedTokens)
            {
                var querySelectedTokens =
                    from PerAddress in tokens
                    from token in PerAddress.Value
                    where token.Key == current.Key
                    select new Dictionary<Address, TaggedTokens<DateTime>> { [PerAddress.Key] = new TaggedTokens<DateTime>{ current }};
                KeyValuePair<Address, TaggedTokens<DateTime>> selectedPair = querySelectedTokens.First().First();

                sortedTokensPerAddress.Add(selectedPair.Key, selectedPair.Value);
            }
//            var querySelectedTokens =
//                from current in pickedTokens
//                from PerAddress in tokens
//                from token in PerAddress.Value
//                where token.Key == current.Key
//                orderby PerAddress ascending
//                select PerAddress;
            return sortedTokensPerAddress;
        }
    }
}
