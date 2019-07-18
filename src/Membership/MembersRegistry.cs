using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using Membership.Actions;
using Action = ContractsCore.Actions.Action;

namespace Membership
{
	// TODO: Delete MembersRegistry class. It's probably useless, because all the needed functionality
	// is already implemented in AddressRegistry. Otherwise it should at least extend it.
	public class MembersRegistry : AclPermittedContract
	{
		private readonly HashSet<Address> membersAddresses;

		public MembersRegistry(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			HashSet<Address> addressesToMembers = null)
			: base(address, registry, permissionManager)
		{
			this.membersAddresses = addressesToMembers ?? new HashSet<Address>();
			this.ConfigurePermissionManager(permissionManager);
		}

		public IEnumerable<Member> GetAllMembers()
		{
			var members = new HashSet<Member>();
			foreach (Address address in this.membersAddresses)
			{
				Member member = this.GetMember(address);
				if (member != null)
				{
					members.Add(member);
				}
			}

			return members;
		}

		public Member GetMember(Address address)
		{
			return this.membersAddresses.Contains(address) ? (this.registry.GetContract(address) as Member) : null;
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

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(RegisterMemberAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UnregisterMemberAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UpdateMemberAction)), this.Address);
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