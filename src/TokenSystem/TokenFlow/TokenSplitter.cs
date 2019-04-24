// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenSplitter<TTokenTagType> : RecipientManager
	{
		public TokenSplitter(
			Address address,
			TokenManager<TTokenTagType> tokenManager)
			: this(address, tokenManager, new List<Address>())
		{
		}

		public TokenSplitter(
			Address address,
			TokenManager<TTokenTagType> tokenManager,
			IList<Address> recipients)
			: base(address, recipients)
		{
			this.TokenManager = tokenManager;

			this.TokenManager.TokensMinted += this.OnTokensMinted;
			this.TokenManager.TokensTransferred += this.OnTokensTransferred;
		}

		protected TokenManager<TTokenTagType> TokenManager { get; }

		protected virtual void OnTokensMinted(object sender, TokensMintedEventArgs<TTokenTagType> mintedEventArgs)
		{
			if (!(sender is TokenManager<TTokenTagType> tokenManagerSender))
			{
				return;
			}

			if (tokenManagerSender.Address.Equals(this.TokenManager.Address) && mintedEventArgs.To.Equals(this.Address))
			{
				this.Split(mintedEventArgs.Amount);
			}
		}

		protected virtual void OnTokensTransferred(
			object sender,
			TokensTransferredEventArgs<TTokenTagType> transferredEventArgs)
		{
			if (!(sender is TokenManager<TTokenTagType> tokenManagerSender) ||
				!tokenManagerSender.Address.Equals(this.TokenManager.Address) ||
				!transferredEventArgs.To.Equals(this.Address))
			{
				return;
			}

			this.Split(transferredEventArgs.Amount);
		}

		protected abstract void Split(BigInteger amount);
	}
}