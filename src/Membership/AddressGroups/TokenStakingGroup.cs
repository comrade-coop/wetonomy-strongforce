using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Permissions;
using Membership.AddressGroups.Actions;
using Action = ContractsCore.Actions.Action;

namespace Membership.AddressGroups
{
	// TODO: Implement TokenStakingGroup. It should be a group which forwards if the caller is staking tokens.
	public class TokenStakingGroup : AddressGroup
	{
		private Address tokenManager;
		private uint requiredStake;

		public TokenStakingGroup(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			uint requiredStake,
			Address tokenManager,
			ISet<Address> members = null)
			: base(address, registry, permissionManager, members)
		{
			this.requiredStake = requiredStake;
			this.tokenManager = tokenManager;
			this.ConfigurePermissionManager(permissionManager);
		}

		protected override bool HandleReceivedAction(Action action)
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

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(
				permissionManager,
				new Permission(typeof(ChangeRequiredStakeAction)),
				this.Address);
		}

		private bool HandleChangeStakeAction(ChangeRequiredStakeAction action)
		{
			this.requiredStake = action.RequiredStake;
			return true;
		}
	}
}