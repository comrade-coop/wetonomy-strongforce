using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Membership.Actions;
using Membership.Exceptions;
using TokenSystem.TokenManagerBase.Actions;
using Action = ContractsCore.Actions.Action;

namespace Membership
{
	public class Member : AclPermittedContract
	{
		private SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensReceivedStrategy;

		private SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensMintedStrategy;

		public Member(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensReceivedStrategy = null,
			SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensMintedStrategy = null)
			: base(address, registry, permissionManager)
		{
			this.addressToTokensReceivedStrategy = addressToTokensReceivedStrategy ??
			                                       new SortedDictionary<Address, ITokensReceivedStrategy>();
			this.addressToTokensMintedStrategy = addressToTokensMintedStrategy ??
			                                     new SortedDictionary<Address, ITokensReceivedStrategy>();
			this.ConfigurePermissionManager(permissionManager);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction receivedAction:
					return this.TokensReceived(receivedAction, ref this.addressToTokensReceivedStrategy);

				case TokensMintedAction mintedAction:
					return this.TokensReceived(mintedAction, ref this.addressToTokensMintedStrategy);

				case AddTokensMintedStrategyAction addAction:
					this.addressToTokensMintedStrategy.Add(addAction.TokenManager, addAction.Strategy);
					return true;

				case AddTokensReceivedStrategyAction addStrategyAction:
					this.addressToTokensReceivedStrategy.Add(
						addStrategyAction.TokenManager,
						addStrategyAction.Strategy);
					return true;

				case RemoveTokensMintedStrategyAction removeAction:
					this.addressToTokensMintedStrategy.Remove(removeAction.TokenManager);
					return true;

				case RemoveTokensReceivedStrategyAction removeAction:
					this.addressToTokensReceivedStrategy.Remove(removeAction.TokenManager);
					return true;

				default:
					return false;
			}
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		private bool TokensReceived(
			TokenAction action,
			ref SortedDictionary<Address, ITokensReceivedStrategy> addressToStrategy)
		{
			if (!addressToStrategy.ContainsKey(action.Sender))
			{
				throw new NotRegisteredReceivedStrategyException(action, this.Address);
			}

			foreach ((IComparable key, BigInteger value) in action.Tokens)
			{
				var result = addressToStrategy[action.Sender]
					.Execute(value, action.Sender, this.Address, key);
				foreach (Action curAction in result)
				{
					this.OnSend(curAction);
				}
			}

			return true;
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(
				permissionManager,
				new Permission(typeof(AddTokensMintedStrategyAction)),
				this.Address);
			this.acl.AddPermission(
				permissionManager,
				new Permission(typeof(RemoveTokensMintedStrategyAction)),
				this.Address);
			this.acl.AddPermission(
				permissionManager,
				new Permission(typeof(AddTokensReceivedStrategyAction)),
				this.Address);
			this.acl.AddPermission(
				permissionManager,
				new Permission(typeof(RemoveTokensMintedStrategyAction)),
				this.Address);
		}
	}
}