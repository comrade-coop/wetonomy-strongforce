// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager;

namespace TokenSystem.TokenFlow
{
	public abstract class OnBurnTokenMinter<TTokenTagType> : RecipientManager
	{
		public OnBurnTokenMinter(Address address, TokenManager<TTokenTagType> tokenManager)
			: this(address, tokenManager, new List<Address>())
		{
		}

		public OnBurnTokenMinter(
			Address address,
			TokenManager<TTokenTagType> tokenManager,
			IList<Address> recipients)
			: base(address, recipients)
		{
			this.TokenManager = tokenManager;
			this.TokenManager.TokensBurned += this.OnTokensBurned;
		}

		public TokenManager<TTokenTagType> TokenManager { get; }

		protected abstract void Mint(BigInteger amount);

		private void OnTokensBurned(object sender, TokensBurnedEventArgs<TTokenTagType> burnArgs)
		{
			if (!(sender is TokenManager<TTokenTagType> tokenManagerSender) ||
				!tokenManagerSender.Address.Equals(this.TokenManager.Address))
			{
				return;
			}

			this.Mint(burnArgs.Amount);
		}
	}
}