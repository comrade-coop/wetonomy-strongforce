using System;
using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public class TokenManager : ITokenManager
    {
        private readonly string symbol;
        private readonly ITokenTagger tokenTagger;

        private IDictionary<Address, IDictionary<string, decimal>> holdersToTaggedBalances;
        private IDictionary<Address, decimal> holdersToBalances;
        private IDictionary<string, decimal> tagsToTotalBalances;
        private decimal totalBalance;

        public TokenManager(string symbol,
            IDictionary<Address, IDictionary<string, decimal>> initialAddressesToBalances,
            ITokenTagger tokenTagger)
        {
            this.symbol = symbol;
            this.tokenTagger = tokenTagger;
            InitialiseBalances(initialAddressesToBalances);
        }

        public TokenManager(string symbol, ITokenTagger tokenTagger) : this(symbol,
            new SortedDictionary<Address, IDictionary<string, decimal>>(), tokenTagger)
        {
        }

        public string Symbol() => symbol;

        public decimal BalanceOf(Address tokenHolder)
            => holdersToBalances.ContainsKey(tokenHolder) ? holdersToBalances[tokenHolder] : 0;


        public IDictionary<string, decimal> TaggedBalanceOf(Address tokenHolder, ITagProperties tagProperties = null)
            => holdersToTaggedBalances.ContainsKey(tokenHolder)
                ? tokenTagger.Pick(holdersToTaggedBalances[tokenHolder], tagProperties)
                : new Dictionary<string, decimal>();

        public decimal TotalBalance()
            => totalBalance;

        public IDictionary<string, decimal> TaggedTotalBalance(ITagProperties tagProperties = null)
            => tokenTagger.Pick(tagsToTotalBalances, tagProperties);

        public void Mint(decimal amount, Address receiver, ITagProperties tagProperties = null)
        {
            RequirePositiveAmount(amount);
            RequireValidAddress(receiver);

            IDictionary<string, decimal> newTokens = tokenTagger.Tag(receiver, amount);
            AddToBalance(newTokens, receiver);
        }

        public void Transfer(decimal amount, Address from, Address to, ITagProperties tagProperties = null)
        {
            RequirePositiveAmount(amount);

            if (from.Equals(to))
            {
                throw new ArgumentException("Addresses can't transfer to themselves");
            }

            IDictionary<string, decimal> tokensToTransfer =
                tokenTagger.Pick(holdersToTaggedBalances[from], amount, tagProperties);
            AddToBalance(tokensToTransfer, to);
            RemoveFromBalance(tokensToTransfer, from);
        }

        public void Burn(decimal amount, Address tokenHolder, ITagProperties tagProperties = null)
        {
            RequirePositiveAmount(amount);

            IDictionary<string, decimal> tokensToBurn =
                tokenTagger.Pick(holdersToTaggedBalances[tokenHolder], amount, tagProperties);
            RemoveFromBalance(tokensToBurn, tokenHolder);
        }

        private void RequirePositiveAmount(decimal tokenAmount)
        {
            if (tokenAmount <= 0)
            {
                throw new NonPositiveTokenAmountException(tokenAmount);
            }
        }

        private void RequireValidAddress(Address address)
        {
            if (address.Equals(new Address(new byte[] { })))
            {
                throw new ArgumentException("Null Address used");
            }
        }

        private void InitialiseBalances(
            IDictionary<Address, IDictionary<string, decimal>> initialHoldersToBalances)
        {
            holdersToTaggedBalances = initialHoldersToBalances;
            holdersToBalances = new SortedDictionary<Address, decimal>();
            tagsToTotalBalances = new SortedDictionary<string, decimal>();
            totalBalance = 0;

            foreach ((Address address, IDictionary<string, decimal> tagsToBalances) in holdersToTaggedBalances)
            {
                foreach ((string tag, decimal amount) in tagsToBalances)
                {
                    tagsToTotalBalances[tag] += amount;
                    holdersToBalances[address] += amount;
                    totalBalance += amount;
                }
            }
        }

        private void AddToBalance(IDictionary<string, decimal> newTokens, Address holder)
        {
            if (!holdersToTaggedBalances.ContainsKey(holder) || !holdersToBalances.ContainsKey(holder))
            {
                holdersToTaggedBalances[holder] = new SortedDictionary<string, decimal>();
                holdersToBalances[holder] = 0;
            }

            foreach ((string tag, decimal amount) in newTokens)
            {
                RequirePositiveAmount(amount);
                if (!holdersToTaggedBalances[holder].ContainsKey(tag))
                {
                    holdersToTaggedBalances[holder][tag] = 0;
                }

                if (!tagsToTotalBalances.ContainsKey(tag))
                {
                    tagsToTotalBalances[tag] = 0;
                }

                UpdateBalances(holder, amount, tag);
            }
        }

        private void RemoveFromBalance(IDictionary<string, decimal> tokensToRemove, Address holder)
        {
            foreach ((string tag, decimal amount) in tokensToRemove)
            {
                RequirePositiveAmount(amount);

                if (!tagsToTotalBalances.ContainsKey(tag))
                {
                    throw new ArgumentException($"There are no tokens with tag {tag}.");
                }

                if (!holdersToTaggedBalances[holder].ContainsKey(tag))
                {
                    throw new InsufficientTokenAmountException(0, amount);
                }

                if (amount > holdersToTaggedBalances[holder][tag])
                {
                    throw new InsufficientTokenAmountException(holdersToTaggedBalances[holder][tag], amount);
                }

                UpdateBalances(holder, -amount, tag);
            }
        }

        private void UpdateBalances(Address holder, decimal amount, string tokenTag)
        {
            holdersToTaggedBalances[holder][tokenTag] += amount;
            holdersToBalances[holder] += amount;
            tagsToTotalBalances[tokenTag] += amount;
            totalBalance += amount;
        }
    }
}