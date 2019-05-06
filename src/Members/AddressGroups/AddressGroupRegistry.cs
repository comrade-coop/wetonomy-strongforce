using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Members.AddressGroups.Actions;
using Action = ContractsCore.Actions.Action;

namespace Members.AddressGroups
{
	public class AddressGroupRegistry : AclPermittedContract
	{
		protected HashSet<Address> groupAddresses;

		protected ContractRegistry registry;

		public AddressGroupRegistry(Address address, ContractRegistry registry, Address permissionManager,
			HashSet<Address> groupAddresses)
			: base(address, registry, permissionManager)
		{
			this.groupAddresses = groupAddresses ?? new HashSet<Address>();
			this.registry = registry;
			this.ConfigurePermissionManager(permissionManager);
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(RegisterGroupAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UnregisterGroupAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UpdateGroupAction)), this.Address);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case RegisterGroupAction registerAction:
					return this.HandleRegisterGroup(registerAction);

				case UnregisterGroupAction unregisterAction:
					return this.HandleUnregisterGroup(unregisterAction);

				/*case UpdateGroupAction updateAction:
					return this.HandleUpdateGroup(updateAction);*/

				default:
					return false;
			}
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

		/*private bool HandleUpdateGroup(UpdateGroupAction action)
		{
			AddressGroup group = action.AddressGroup;
			var g = this.registry.GetContractForAddress(group.Address);
			this.groupAddresses[group.Address] = group;
			return true;
		}*/

		public AddressGroup GetGroup(Address address)
		{
			return this.groupAddresses.Contains(address) ? (this.registry.GetContract(address) as AddressGroup) : null;
		}

		public HashSet<AddressGroup> GetAllGroups()
		{
			var members = new HashSet<AddressGroup>();
			foreach (var address in groupAddresses)
			{
				members.Add(this.registry.GetContract(address) as AddressGroup);
			}

			return members;
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new System.NotImplementedException();
		}

		protected override object GetState()
		{
			throw new System.NotImplementedException();
		}
	}
}