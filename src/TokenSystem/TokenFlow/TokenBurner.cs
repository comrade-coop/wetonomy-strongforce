// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using TokenSystem.TokenManagerBase.Actions;

namespace TokenSystem.TokenFlow
{
	public class TokenBurner : Contract
	{
		public TokenBurner(Address address, Address tokenManager)
			: base(address)
		{
			this.TokenManager = tokenManager;
		}

		public Address TokenManager { get; }

		protected override object GetState()
		{
			return new object();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensMintedAction mintedAction:
					this.Burn(mintedAction.Tokens.TotalBalance);
					return true;
				case TokensReceivedAction receivedAction:
					this.Burn(receivedAction.Tokens.TotalBalance);
					return true;
				default:
					return false;
			}
		}

		private void Burn(BigInteger amount)
		{
			var burnActionTo = new BurnAction(
				string.Empty,
				this.TokenManager,
				amount);
			this.OnSend(burnActionTo);
		}
	}
}