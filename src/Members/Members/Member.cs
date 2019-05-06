using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Members.Actions;
using Members.Exeptions;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using Action = ContractsCore.Actions.Action;

namespace Members
{
	public class Member : AclPermittedContract
	{
		protected SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensReceivedStrategy;

		protected SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensMintedStrategy;

		public Member(Address address, ContractRegistry registry, Address permissionManager,
			SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensReceivedStrategy = null,
			SortedDictionary<Address, ITokensReceivedStrategy> addressToTokensMintedStrategy = null)
			: base(address, registry, permissionManager)
		{
			this.addressToTokensReceivedStrategy = addressToTokensReceivedStrategy ?? new SortedDictionary<Address, ITokensReceivedStrategy>();
			this.addressToTokensMintedStrategy = addressToTokensMintedStrategy ?? new SortedDictionary<Address, ITokensReceivedStrategy>();
			this.ConfigurePermissionManager(permissionManager);
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(AddTokensMintedStrategyAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(RemoveTokensMintedStrategyAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(AddTokensReceivedStrategyAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(RemoveTokensMintedStrategyAction)), this.Address);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction receivedAction:
					return this.HandleTokensTransferred(receivedAction);

				case TokensMintedAction mintedAction:
					return this.HandleTokensMinted(mintedAction);

				case AddTokensMintedStrategyAction addStrategyAction:
					return this.HandleAddTokensMintedStrategy(addStrategyAction);

				case AddTokensReceivedStrategyAction addStrategyAction:
					return this.HandleAddTokensReceivedStrategy(addStrategyAction);

				case RemoveTokensMintedStrategyAction addStrategyAction:
					return this.HandleRemoveTokensMintedStrategy(addStrategyAction);

				case RemoveTokensReceivedStrategyAction addStrategyAction:
					return this.HandleRemoveTokensReceivedStrategy(addStrategyAction);

				default:
					return false;
			}
		}

		protected bool HandleAddTokensMintedStrategy(AddTokensMintedStrategyAction action)
		{
			this.addressToTokensMintedStrategy.Add(action.TokenManager, action.Strategy);
			return true;
		}

		protected bool HandleAddTokensReceivedStrategy(AddTokensReceivedStrategyAction action)
		{
			this.addressToTokensReceivedStrategy.Add(action.TokenManager, action.Strategy);
			return true;
		}

		protected bool HandleRemoveTokensMintedStrategy(RemoveTokensMintedStrategyAction action)
		{
			this.addressToTokensMintedStrategy.Remove(action.TokenManager);
			return true;
		}

		protected bool HandleRemoveTokensReceivedStrategy(RemoveTokensReceivedStrategyAction action)
		{
			this.addressToTokensReceivedStrategy.Remove(action.TokenManager);
			return true;
		}

		protected virtual bool HandleTokensTransferred(TokensReceivedAction action)
		{
			return this.TokensReceived(action, ref this.addressToTokensReceivedStrategy);
		}

		protected virtual bool HandleTokensMinted(TokensMintedAction action)
		{
			return this.TokensReceived(action, ref this.addressToTokensMintedStrategy);
		}

		protected bool TokensReceived(TokenAction action,
			ref SortedDictionary<Address, ITokensReceivedStrategy> addresstoStrategy)
		{
			if (!addresstoStrategy.ContainsKey(action.Sender))
			{
				throw new NotRegisteredReceivedStrategyExeption(action, this.Address);
			}

			foreach (var token in action.Tokens)
			{
				var result = addresstoStrategy[action.Sender].OnTokensReceived(token.Value, action.Sender, this.Address, token.Key);
				foreach (var curAction in result)
				{
					this.OnSend(curAction);
				}
			}

			return true;
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		public static bool IsAssignableToGenericType(Type givenType, Type genericType)
		{
			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
					return true;
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return true;

			Type baseType = givenType.BaseType;
			if (baseType == null) return false;

			return IsAssignableToGenericType(baseType, genericType);
		}
	}
}