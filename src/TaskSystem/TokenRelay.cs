using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Contracts;
using TaskSystem.Actions;
using TaskSystem.Exceptions;
using TokenSystem.TokenManagerBase.Actions;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem
{
	public abstract class TokenRelay : AclPermittedContract
	{
		public Address TokenReceiver { get; private set; }

		public IDictionary<Address, BigInteger> TokenManagersToBalances { get; }

		public TokenRelay(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			Address receiver,
			IDictionary<Address, BigInteger> tokenManagers = null)
			: base(address, registry, permissionManager)
		{
			if (receiver.Equals(this.Address))
			{
				throw new UnallowedRewardReceiverException(this, receiver);
			}

			this.TokenReceiver = receiver;
			this.TokenManagersToBalances = tokenManagers ?? new SortedDictionary<Address, BigInteger>();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case ChnageTaskRewardReceiverAction changeAction:
					return this.HandleRewardReceiverChange(changeAction);

				case TokensReceivedAction receivedAction:
					return this.HandleTokensReceived(receivedAction);

				case TokensMintedAction mintedAction:
					return this.HandleTokensReceived(mintedAction);

				case DumpTokensAction dumpAction:
					return this.HandleDumpTokens(dumpAction);

				default: return false;
			}
		}

		protected virtual bool HandleTokensReceived(TokenAction action)
		{
			if (this.TokenManagersToBalances.ContainsKey(action.Origin))
			{
				this.TokenManagersToBalances[action.Origin] += action.Tokens.TotalBalance;
			}
			else
			{
				this.TokenManagersToBalances.Add(action.Origin, action.Tokens.TotalBalance);
			}

			return true;
		}

		protected virtual bool HandleDumpTokens(DumpTokensAction action)
		{
			foreach (var managerToBalance in this.TokenManagersToBalances)
			{
				if (managerToBalance.Value > 0)
				{
					var transfer = new TransferAction(
						string.Empty,
						managerToBalance.Key,
						managerToBalance.Value,
						this.Address,
						this.TokenReceiver);
					this.OnSend(transfer);
				}
			}

			return true;
		}

		private bool HandleRewardReceiverChange(ChnageTaskRewardReceiverAction action)
		{
			if (action.RewardReceiver.CompareTo(action.Target) == 0)
			{
				throw new UnallowedRewardReceiverException(this, action.RewardReceiver);
			}

			this.TokenReceiver = action.RewardReceiver;
			return true;
		}
	}
}