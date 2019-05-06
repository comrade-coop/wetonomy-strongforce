using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Members.Actions;
using Action = ContractsCore.Actions.Action;

namespace Members
{
	public class MemberRegistry : AclPermittedContract
	{
		protected HashSet<Address> membersAddresses;

		protected ContractRegistry registry;

		public MemberRegistry(Address address, ContractRegistry registry, Address permissionManager)
			: this(address, registry, permissionManager, null)
		{
		}

		public MemberRegistry(Address address, ContractRegistry registry, Address permissionManager,
			HashSet<Address> addressesToMembers)
			: base(address, registry, permissionManager)
		{
			this.membersAddresses = addressesToMembers ?? new HashSet<Address>();
			this.registry = registry;
			this.ConfigurePermissionManager(permissionManager);
		}

		public HashSet<Member> GetAllMembers()
		{
			var members = new HashSet<Member>();
			foreach (var address in membersAddresses)
			{
				members.Add(this.registry.GetContract(address) as Member);
			}

			return members;
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(RegisterMemberAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UnregisterMemberAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UpdateMemberAction)), this.Address);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case RegisterMemberAction registerAction:
					return this.HandleRegisterMember(registerAction);

				case UnregisterMemberAction unregisterAction:
					return this.HandleUnregisterMember(unregisterAction);

				/*case UpdateMemberAction<Member> updateAction:
					return this.HandleUpdateMember(updateAction);*/

				default:
					return false;
			}
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		public Member GetMember(Address address)
		{
			return this.membersAddresses.Contains(address) ? (this.registry.GetContract(address) as Member) : null;
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		private bool HandleRegisterMember(RegisterMemberAction action)
		{
			Member member = action.Member;
			try
			{
				this.registry.GetContract(member.Address);
			}
			catch (KeyNotFoundException)
			{
				this.registry.RegisterContract(member);
			}

			return this.membersAddresses.Add(member.Address);
		}

		private bool HandleUnregisterMember(UnregisterMemberAction action)
		{
			return this.membersAddresses.Remove(action.MemberAddress);
		}

		/*private bool HandleUpdateMember(UpdateMemberAction<Member> action)
		{
			Member member = action.Member;
			this.membersAddresses[member.Address] = member;
			return true;
		}*/
	}
}