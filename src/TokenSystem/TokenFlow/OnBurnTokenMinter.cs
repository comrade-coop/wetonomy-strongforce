// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManagerBase;

namespace TokenSystem.TokenFlow
{
	public abstract class OnBurnTokenMinter : RecipientManager
	{
		public OnBurnTokenMinter(Address address, TokenManager tokenManager)
			: this(address, tokenManager, new HashSet<Address>())
		{
		}

		public OnBurnTokenMinter(
			Address address,
			TokenManager tokenManager,
			ISet<Address> recipients)
			: base(address, recipients)
		{
			this.TokenManager = tokenManager;
			this.TokenManager.TokensBurned += this.OnTokensBurned;
		}

		public TokenManager TokenManager { get; }

		protected abstract void Mint(BigInteger amount);

		private void OnTokensBurned(object sender, TokensBurnedEventArgs burnArgs)
		{
			if (!(sender is TokenManager tokenManagerSender) ||
				!tokenManagerSender.Address.Equals(this.TokenManager.Address))
			{
				return;
			}

			this.Mint(burnArgs.Tokens.TotalBalance);
		}
	}
}