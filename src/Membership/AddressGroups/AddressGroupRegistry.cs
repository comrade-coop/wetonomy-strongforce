using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Membership.AddressGroups.Actions;
using Action = ContractsCore.Actions.Action;

namespace Membership.AddressGroups
{
	public class AddressGroupRegistry : AclPermittedContract
	{
		private readonly ISet<Address> groupAddresses;

		public AddressGroupRegistry(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			ISet<Address> groupAddresses)
			: base(address, registry, permissionManager)
		{
			this.groupAddresses = groupAddresses ?? new HashSet<Address>();
			this.ConfigurePermissionManager(permissionManager);
		}

		public ISet<AddressGroup> GetAllGroups()
		{
			var members = new HashSet<AddressGroup>();
			foreach (Address address in this.groupAddresses)
			{
				members.Add(this.registry.GetContract(address) as AddressGroup);
			}

			return members;
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case RegisterGroupAction registerAction:
					return this.HandleRegisterGroup(registerAction);
				case UnregisterGroupAction unregisterAction:
					return this.HandleUnregisterGroup(unregisterAction);
				default:
					return false;
			}
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new System.NotImplementedException();
		}

		protected override object GetState()
		{
			throw new System.NotImplementedException();
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(RegisterGroupAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UnregisterGroupAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UpdateGroupAction)), this.Address);
		}

		private bool HandleRegisterGroup(RegisterGroupAction action)
		{
			AddressGroup group = action.AddressGroup;
			try
			{
				this.registry.GetContract(group.Address);
			}
			catch (KeyNotFoundException)
			{
				this.registry.RegisterContract(group);
			}

			return this.groupAddresses.Add(group.Address);
		}

		private bool HandleUnregisterGroup(UnregisterGroupAction action)
		{
			return this.groupAddresses.Remove(action.AddressGroup);
		}
	}
}