using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using TokenSystem.Exceptions;
using TokenSystem.TokenEventArgs;
using Action = ContractsCore.Actions.Action;

namespace TokenSystem.Tokens
{
	public class TokenManager : PermittedContract, ITokenManager
	{
		private readonly string symbol;
		private readonly ITokenTagger tokenTagger;
		private readonly ITaggedTokenPickStrategy defaultPickStrategy;

		private IDictionary<Address, TaggedTokens> holdersToTaggedBalances;
		private IDictionary<Address, decimal> holdersToBalances;
		private TaggedTokens tagsToTotalBalances;
		private decimal totalBalance;

		public event EventHandler<TokensMintedEventArgs> TokensMinted;
		public event EventHandler<TokensTransferredEventArgs> TokensTransferred;
		public event EventHandler<TokensBurnedEventArgs> TokensBurned;

		public TokenManager(
			Address address,
			Address permissionManager,
			string symbol,
			IDictionary<Address, TaggedTokens> initialAddressesToBalances,
			ITokenTagger tokenTagger, ITaggedTokenPickStrategy defaultPickStrategy)
			: base(address, permissionManager)
		{
			this.symbol = symbol;
			this.tokenTagger = tokenTagger;
			this.defaultPickStrategy = defaultPickStrategy;
			this.InitialiseBalances(initialAddressesToBalances);
		}

		public TokenManager(
			Address address,
			Address permissionManager,
			string symbol,
			ITokenTagger tokenTagger,
			ITaggedTokenPickStrategy defaultPickStrategy)
			: this(address,
				permissionManager,
				symbol,
				new SortedDictionary<Address, TaggedTokens>(),
				tokenTagger,
				defaultPickStrategy)
		{
		}

		public string Symbol() => this.symbol;

		public decimal BalanceOf(Address tokenHolder)
			=> this.holdersToBalances.ContainsKey(tokenHolder) ? this.holdersToBalances[tokenHolder] : 0;


		public TaggedTokens TaggedBalanceOf(Address tokenHolder)
			=> this.holdersToBalances.ContainsKey(tokenHolder)
				? this.holdersToTaggedBalances[tokenHolder]
				: new TaggedTokens();


		public decimal TotalBalance() => this.totalBalance;

		public TaggedTokens TaggedTotalBalance() => this.tagsToTotalBalances;

		private void Mint(decimal amount, Address to)
		{
			RequirePositiveAmount(amount);
			RequireValidAddress(to);

			TaggedTokens newTokens = this.tokenTagger.Tag(to, amount);
			this.AddToBalance(newTokens, to);

			var tokensMintedArgs = new TokensMintedEventArgs(amount, newTokens, to);
			this.OnTokensMinted(tokensMintedArgs);
		}

		private void Transfer(decimal amount, Address from, Address to, ITaggedTokenPickStrategy pickStrategy = null)
		{
			RequirePositiveAmount(amount);
			RequireValidAddress(from);
			RequireValidAddress(to);

			if (from.Equals(to))
			{
				throw new ArgumentException("Addresses can't transfer to themselves");
			}

			pickStrategy = pickStrategy ?? this.defaultPickStrategy;

			TaggedTokens tokensToTransfer =
				pickStrategy.Pick(this.holdersToTaggedBalances[from], amount);
			this.AddToBalance(tokensToTransfer, to);
			this.RemoveFromBalance(tokensToTransfer, from);

			var transferArgs = new TokensTransferredEventArgs(amount, tokensToTransfer, from, to);
			this.OnTokensTransferred(transferArgs);
		}

		private void Burn(decimal amount, Address from, ITaggedTokenPickStrategy pickStrategy = null)
		{
			RequirePositiveAmount(amount);

			pickStrategy = pickStrategy ?? this.defaultPickStrategy;

			IDictionary<string, decimal> tokensToBurn =
				pickStrategy.Pick(this.holdersToTaggedBalances[from], amount);
			this.RemoveFromBalance(tokensToBurn, from);

			var burnArgs = new TokensBurnedEventArgs(amount, tokensToBurn, from);
			this.OnTokensBurned(burnArgs);
		}

		private static void RequirePositiveAmount(decimal tokenAmount)
		{
			if (tokenAmount <= 0)
			{
				throw new NonPositiveTokenAmountException(tokenAmount);
			}
		}

		private static void RequireValidAddress(Address address)
		{
			if (address == null)
			{
				throw new ArgumentNullException(nameof(address));
			}
		}

		private void InitialiseBalances(
			IDictionary<Address, TaggedTokens> initialHoldersToBalances)
		{
			this.holdersToTaggedBalances = initialHoldersToBalances;
			this.holdersToBalances = new SortedDictionary<Address, decimal>();
			this.tagsToTotalBalances = new TaggedTokens();
			this.totalBalance = 0;

			foreach ((Address address, IDictionary<string, decimal> tagsToBalances) in this.holdersToTaggedBalances)
			{
				foreach ((string tag, decimal amount) in tagsToBalances)
				{
					this.tagsToTotalBalances[tag] += amount;
					this.holdersToBalances[address] += amount;
					this.totalBalance += amount;
				}
			}
		}

		private void AddToBalance(IDictionary<string, decimal> newTokens, Address holder)
		{
			if (!this.holdersToTaggedBalances.ContainsKey(holder) || !this.holdersToBalances.ContainsKey(holder))
			{
				this.holdersToTaggedBalances[holder] = new TaggedTokens();
				this.holdersToBalances[holder] = 0;
			}

			foreach ((string tag, decimal amount) in newTokens)
			{
				RequirePositiveAmount(amount);
				if (!this.holdersToTaggedBalances[holder].ContainsKey(tag))
				{
					this.holdersToTaggedBalances[holder][tag] = 0;
				}

				if (!this.tagsToTotalBalances.ContainsKey(tag))
				{
					this.tagsToTotalBalances[tag] = 0;
				}

				this.UpdateBalances(holder, amount, tag);
			}
		}

		private void RemoveFromBalance(IDictionary<string, decimal> tokensToRemove, Address holder)
		{
			foreach ((string tag, decimal amount) in tokensToRemove)
			{
				RequirePositiveAmount(amount);

				if (!this.tagsToTotalBalances.ContainsKey(tag))
				{
					throw new ArgumentException($"There are no tokens with tag {tag}.");
				}

				if (!this.holdersToTaggedBalances[holder].ContainsKey(tag))
				{
					throw new InsufficientTokenAmountException(0, amount);
				}

				if (amount > this.holdersToTaggedBalances[holder][tag])
				{
					throw new InsufficientTokenAmountException(this.holdersToTaggedBalances[holder][tag], amount);
				}

				this.UpdateBalances(holder, -amount, tag);
			}
		}

		private void UpdateBalances(Address holder, decimal amount, string tokenTag)
		{
			this.holdersToTaggedBalances[holder][tokenTag] += amount;
			this.holdersToBalances[holder] += amount;
			this.tagsToTotalBalances[tokenTag] += amount;
			this.totalBalance += amount;
		}

		protected virtual void OnTokensMinted(TokensMintedEventArgs e)
		{
			this.TokensMinted?.Invoke(this, e);
		}

		protected virtual void OnTokensTransferred(TokensTransferredEventArgs e)
		{
			this.TokensTransferred?.Invoke(this, e);
		}

		protected virtual void OnTokensBurned(TokensBurnedEventArgs e)
		{
			this.TokensBurned?.Invoke(this, e);
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		protected override bool HandleAcceptedAction(Action action)
		{
			switch (action)
			{
				case MintAction mintAction:
					this.Mint(mintAction.Amount, mintAction.To);
					return true;

				case TransferAction transferAction:
					this.Transfer(
						transferAction.Amount,
						transferAction.From,
						transferAction.To,
						transferAction.PickStrategy);
					return true;

				case BurnAction burnAction:
					this.Burn(burnAction.Amount, burnAction.From, burnAction.PickStrategy);
					return true;

				default:
					return false;
			}
		}
	}
}