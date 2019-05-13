using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Members.AddressGroups.Actions;
using Action = ContractsCore.Actions.Action;

namespace Members.AddressGroups
{
	public class AddressGroup : AclPermittedContract, IAddressGroup
	{
		protected HashSet<Address> members;

		public AddressGroup(Address address, ContractRegistry registry, Address permissionManager, HashSet<Address> members = null)
			: base(address, registry, permissionManager)
		{
			this.members = members ?? new HashSet<Address>();
			this.ConfigurePermissionManager(permissionManager);
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(RegisterGroupMemberAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UnregisterGroupMemberAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(ChangeRequiredStakeAction)), this.Address);
		}

		public virtual HashSet<Address> GetAllMembers()
		{
			return this.members;
		}

		public virtual bool IsMember(Address address)
		{
			return this.members.Contains(address);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{

				case RegisterGroupMemberAction registerAction:
					return this.HandleAddMemberAction(registerAction);

				case UnregisterGroupMemberAction unregisterAction:
					return this.HandleRemoveMemberAction(unregisterAction);

				default:
					return false;
			}
		}

		protected virtual bool HandleAddMemberAction(RegisterGroupMemberAction action)
		{
			this.members.Add(action.MemberAddress);
			return true;
		}

		protected virtual bool HandleRemoveMemberAction(UnregisterGroupMemberAction action)
		{
			return this.members.Remove(action.MemberAddress);
		}

		protected override void BulletTaken(List<Stack<Address>> ways, ContractsCore.Actions.Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}
	}
}