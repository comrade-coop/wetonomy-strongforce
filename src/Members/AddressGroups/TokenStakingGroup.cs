using System;
using System.Collections.Generic;
using System.Text;
using ContractsCore;
using ContractsCore.Permissions;
using Members.AddressGroups.Actions;
using TokenSystem.TokenManagerBase;
using Action = ContractsCore.Actions.Action;

namespace Members.AddressGroups
{
	public class TokenStakingGroup : AddressGroup
	{
		protected ITokenManager tokenManager;
		protected uint requiredStake;

		public TokenStakingGroup(Address address, ContractRegistry registry, Address permissionManager,
			uint requiredStake, ITokenManager tokenManager, HashSet<Address> members = null)
			: base(address, registry, permissionManager, members)
		{
			this.requiredStake = requiredStake;
			this.tokenManager = tokenManager;
		}

		protected virtual bool HandleReceivedAction(Action action)
		{
			if (base.HandleReceivedAction(action))
			{
				return true;
			}

			switch (action)
			{
				case ChangeRequiredStakeAction changeStakeAction:
					return this.HandleChangeStakeAction(changeStakeAction);

				default:
					return false;
			}
		}

		protected bool HandleChangeStakeAction(ChangeRequiredStakeAction action)
		{
			this.requiredStake = action.RequiredStake;
			return true;
		}
	}
}