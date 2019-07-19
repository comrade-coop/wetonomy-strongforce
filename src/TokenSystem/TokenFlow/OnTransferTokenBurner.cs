// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using ContractsCore.Events;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public class OnTransferTokenBurner : Contract
	{
		public OnTransferTokenBurner(Address address, TokenManager tokenManager)
			: base(address)
		{
			this.TokenManager = tokenManager;
			this.TokenManager.TokensTransferred += this.OnTokensTransferred;
		}

		public TokenManager TokenManager { get; }

		protected override object GetState()
		{
			return new object();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			return false;
		}

		private void Burn(BigInteger amount, Address burnedAddress)
		{
			var burnActionTo = new BurnAction(
				string.Empty,
				this.TokenManager.Address,
				amount,
				burnedAddress);

			this.OnSend(burnActionTo);
		}

		private void OnTokensTransferred(object sender, TokensTransferredEventArgs transferredEventArgs)
		{
			if (!(sender is TokenManager tokenManagerSender) ||
				!tokenManagerSender.Address.Equals(this.TokenManager.Address))
			{
				return;
			}

			this.Burn(transferredEventArgs.Tokens.TotalBalance, transferredEventArgs.To);
		}
	}
}