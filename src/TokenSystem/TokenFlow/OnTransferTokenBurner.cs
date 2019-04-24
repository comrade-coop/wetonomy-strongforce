// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using ContractsCore.Events;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;

namespace TokenSystem.TokenFlow
{
	public class OnTransferTokenBurner<TTokenTagType> : Contract
	{
		public OnTransferTokenBurner(Address address, TokenManager<TTokenTagType> tokenManager)
			: base(address)
		{
			this.TokenManager = tokenManager;
			this.TokenManager.TokensTransferred += this.OnTokensTransferred;
		}

		public TokenManager<TTokenTagType> TokenManager { get; }

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
			var burnActionTo = new BurnAction<TTokenTagType>(
				string.Empty,
				this.Address,
				this.Address,
				this.TokenManager.Address,
				amount,
				burnedAddress);

			this.OnSend(new ActionEventArgs(burnActionTo));
		}

		private void OnTokensTransferred(object sender, TokensTransferredEventArgs<TTokenTagType> transferredEventArgs)
		{
			if (!(sender is TokenManager<TTokenTagType> tokenManagerSender) ||
				!tokenManagerSender.Address.Equals(this.TokenManager.Address))
			{
				return;
			}

			this.Burn(transferredEventArgs.Amount, transferredEventArgs.To);
		}
	}
}