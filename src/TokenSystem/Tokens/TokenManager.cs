using System;
using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.TokenEventArgs;

namespace TokenSystem.Tokens
{
	public class TokenManager<TTagType> : ITokenManager<TTagType>
	{
		private readonly ITokenTagger<TTagType> tokenTagger;
		private readonly ITokenPicker<TTagType> defaultTokenPicker;


		private readonly IDictionary<Address, TaggedTokens<TTagType>> holdersToBalances;
		private readonly TaggedTokens<TTagType> totalBalance;

		public event EventHandler<TokensMintedEventArgs<TTagType>> TokensMinted;
		public event EventHandler<TokensTransferredEventArgs<TTagType>> TokensTransferred;
		public event EventHandler<TokensBurnedEventArgs<TTagType>> TokensBurned;

		public TokenManager(
			string symbol,
			ITokenTagger<TTagType> tokenTagger,
			ITokenPicker<TTagType> defaultTokenPicker)
		{
			this.tokenTagger = tokenTagger;
			this.holdersToBalances = new Dictionary<Address, TaggedTokens<TTagType>>();
			this.totalBalance = new TaggedTokens<TTagType>();
			this.defaultTokenPicker = defaultTokenPicker;
		}

		public TaggedTokens<TTagType> TaggedBalanceOf(Address tokenHolder)
			=> this.holdersToBalances.ContainsKey(tokenHolder)
				? this.holdersToBalances[tokenHolder]
				: new TaggedTokens<TTagType>();

		public TaggedTokens<TTagType> TaggedTotalBalance() => this.totalBalance;

		public void Mint(decimal amount, Address to)
		{
			if (to == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			TokensUtility.RequirePositiveAmount(amount);

			TaggedTokens<TTagType> newTokens = this.tokenTagger.Tag(to, amount);
			this.AddToBalances(newTokens, to);

			var tokensMintedArgs = new TokensMintedEventArgs<TTagType>(amount, newTokens, to);
			this.OnTokensMinted(tokensMintedArgs);
		}

		public void Transfer(decimal amount, Address from, Address to, ITokenPicker<TTagType> customPicker = null)
		{
			if (from == null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			if (to == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			if (from.Equals(to))
			{
				throw new ArgumentException("Addresses can't transfer to themselves");
			}

			TokensUtility.RequirePositiveAmount(amount);

			customPicker = customPicker ?? this.defaultTokenPicker;

			TaggedTokens<TTagType> tokensToTransfer = customPicker.Pick(this.holdersToBalances[from], amount);
			this.RemoveFromBalances(tokensToTransfer, from);
			this.AddToBalances(tokensToTransfer, to);

			var transferArgs = new TokensTransferredEventArgs<TTagType>(amount, tokensToTransfer, from, to);
			this.OnTokensTransferred(transferArgs);
		}

		public void Burn(decimal amount, Address from, ITokenPicker<TTagType> customPicker = null)
		{
			if (from == null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			TokensUtility.RequirePositiveAmount(amount);

			customPicker = customPicker ?? this.defaultTokenPicker;

			TaggedTokens<TTagType> tokensToBurn = customPicker.Pick(this.holdersToBalances[from], amount);
			this.holdersToBalances[from].RemoveFromBalance(tokensToBurn);

			var burnArgs = new TokensBurnedEventArgs<TTagType>(amount, tokensToBurn, from);
			this.OnTokensBurned(burnArgs);
		}

		private void AddToBalances(TaggedTokens<TTagType> tokens, Address holder)
		{
			if (!this.holdersToBalances.ContainsKey(holder))
			{
				this.holdersToBalances[holder] = new TaggedTokens<TTagType>();
			}

			this.holdersToBalances[holder].AddToBalance(tokens);
			this.totalBalance.AddToBalance(tokens);
		}

		private void RemoveFromBalances(TaggedTokens<TTagType> tokens, Address holder)
		{
			this.holdersToBalances[holder].RemoveFromBalance(tokens);
			this.totalBalance.RemoveFromBalance(tokens);
		}

		protected virtual void OnTokensMinted(TokensMintedEventArgs<TTagType> e)
		{
			this.TokensMinted?.Invoke(this, e);
		}

		protected virtual void OnTokensTransferred(TokensTransferredEventArgs<TTagType> e)
		{
			this.TokensTransferred?.Invoke(this, e);
		}

		protected virtual void OnTokensBurned(TokensBurnedEventArgs<TTagType> e)
		{
			this.TokensBurned?.Invoke(this, e);
		}
	}
}