using System;
using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;
using TokenSystem.TokenEventArgs;

namespace TokenSystem.Tokens
{
    public class TokenManager<T> : ITokenManager<T>
    {
        private readonly string symbol;
        private readonly ITokenTagger<T> tokenTagger;

        private IDictionary<Address, TaggedTokens<T>> holdersToTaggedBalances; 
        private IDictionary<Address, decimal> holdersToBalances;
        private TaggedTokens<T> tagsToTotalTokenses;
        private decimal totalBalance;

        public event EventHandler<TokensMintedEventArgs<T>> TokensMinted;
        public event EventHandler<TokensTransferredEventArgs<T>> TokensTransferred;
        public event EventHandler<TokensBurnedEventArgs<T>> TokensBurned;

        public TokenManager(string symbol,
            IDictionary<Address, TaggedTokens<T>> initialAddressesToBalances,
            ITokenTagger<T> tokenTagger)
        {
            this.symbol = symbol;
            this.tokenTagger = tokenTagger;
            this.InitialiseBalances(initialAddressesToBalances);
        }

        public TokenManager(string symbol, ITokenTagger<T> tokenTagger) : this(symbol,
            new SortedDictionary<Address, TaggedTokens<T>>(), tokenTagger)
        {
        }

        public string Symbol() => this.symbol;

        public decimal BalanceOf(Address tokenHolder)
            =>
                this.holdersToBalances.ContainsKey(tokenHolder) ? this.holdersToBalances[tokenHolder] : 0;


        public TaggedTokens<T> TaggedBalanceOf(Address tokenHolder, object tagProps = null)
            =>
                this.holdersToTaggedBalances.ContainsKey(tokenHolder)
                    ? this.tokenTagger.Pick(this.holdersToTaggedBalances[tokenHolder], tagProps)
                    : new TaggedTokens<T>();

        public decimal TotalBalance()
            =>
                this.totalBalance;

        public TaggedTokens<T> TaggedTotalBalance(object tagProps = null)
            =>
                this.tokenTagger.Pick(this.tagsToTotalTokenses, tagProps);

        public void Mint(decimal amount, Address to, object tagProps = null)
        {
            this.RequirePositiveAmount(amount);
            this.RequireValidAddress(to);

            TaggedTokens<T> newTokens = this.tokenTagger.Tag(to, amount);
            this.AddToBalance(newTokens, to);

            var tokensMintedArgs = new TokensMintedEventArgs<T>(amount, newTokens, to);
            this.OnTokensMinted(tokensMintedArgs);
        }

        public void Transfer(decimal amount, Address from, Address to, object tagProps = null)
        {
            this.RequirePositiveAmount(amount);
            this.RequireValidAddress(from);
            this.RequireValidAddress(to);

            if (from.Equals(to))
            {
                throw new ArgumentException("Addresses can't transfer to themselves");
            }

            TaggedTokens<T> tokensToTransfer =
                this.tokenTagger.Pick(this.holdersToTaggedBalances[from], amount, tagProps);
            this.AddToBalance(tokensToTransfer, to);
            this.RemoveFromBalance(tokensToTransfer, from);

            var transferArgs = new TokensTransferredEventArgs<T>(amount, tokensToTransfer, from, to);
            this.OnTokensTransferred(transferArgs);
        }

        public void Burn(decimal amount, Address from, object tagProps = null)
        {
            this.RequirePositiveAmount(amount);

            TaggedTokens<T> tokensToBurn =
                this.tokenTagger.Pick(this.holdersToTaggedBalances[from], amount, tagProps);
            this.RemoveFromBalance(tokensToBurn, from);

            var burnArgs = new TokensBurnedEventArgs<T>(amount, tokensToBurn, from);
            this.OnTokensBurned(burnArgs);
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
            if (address.Equals(new Address()))
            {
                throw new ArgumentException("Null Address used");
            }
        }

        private void InitialiseBalances(
            IDictionary<Address, TaggedTokens<T>> initialHoldersToBalances)
        {
            this.holdersToTaggedBalances = initialHoldersToBalances;
            this.holdersToBalances = new SortedDictionary<Address, decimal>();
            this.tagsToTotalTokenses = new TaggedTokens<T>();
            this.totalBalance = 0;

            foreach ((Address address, TaggedTokens<T> taggedBalance) in this.holdersToTaggedBalances)
            {
                foreach ((T tag, decimal amount) in taggedBalance)
                {
                    this.tagsToTotalTokenses[tag] += amount;
                    this.holdersToBalances[address] += amount;
                    this.totalBalance += amount;
                }
            }
        }

        private void AddToBalance(TaggedTokens<T> newTokens, Address holder)
        {
            if (!this.holdersToTaggedBalances.ContainsKey(holder) || !this.holdersToBalances.ContainsKey(holder))
            {
                this.holdersToTaggedBalances[holder] = new TaggedTokens<T>();
                this.holdersToBalances[holder] = 0;
            }

            foreach ((T tag, decimal amount) in newTokens)
            {
                this.RequirePositiveAmount(amount);
                if (!this.holdersToTaggedBalances[holder].ContainsKey(tag))
                {
                    this.holdersToTaggedBalances[holder][tag] = 0;
                }

                if (!this.tagsToTotalTokenses.ContainsKey(tag))
                {
                    this.tagsToTotalTokenses[tag] = 0;
                }

                this.UpdateBalances(holder, amount, tag);
            }
        }

        private void RemoveFromBalance(TaggedTokens<T> tokensToRemove, Address holder)
        {
            foreach ((T tag, decimal amount) in tokensToRemove)
            {
                this.RequirePositiveAmount(amount);

                if (!this.tagsToTotalTokenses.ContainsKey(tag))
                {
                    throw new ArgumentException($"There are no tokens with tag {tag}.");
                }

                if (!this.holdersToTaggedBalances[holder].ContainsKey(tag))
                {
                    throw new InsufficientTokenAmountException(0, amount);
                }

                if (amount > this.holdersToTaggedBalances[holder][tag])
                {
                    throw new InsufficientTokenAmountException(
                        this.holdersToTaggedBalances[holder][tag], amount);
                }

                this.UpdateBalances(holder, -amount, tag);
            }
        }

        private void UpdateBalances(Address holder, decimal amount, T tokenTag)
        {
            this.holdersToTaggedBalances[holder][tokenTag] += amount;
            this.holdersToBalances[holder] += amount;
            this.tagsToTotalTokenses[tokenTag] += amount;
            this.totalBalance += amount;
        }

        protected virtual void OnTokensMinted(TokensMintedEventArgs<T> e)
        {
            this.TokensMinted?.Invoke(this, e);
        }

        protected virtual void OnTokensTransferred(TokensTransferredEventArgs<T> e)
        {
            this.TokensTransferred?.Invoke(this, e);
        }

        protected virtual void OnTokensBurned(TokensBurnedEventArgs<T> e)
        {
            this.TokensBurned?.Invoke(this, e);
        }
    }
}