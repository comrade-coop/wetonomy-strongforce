using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using Members.Actions;
using Action = ContractsCore.Actions.Action;

namespace Members
{
	public class MemberRegistry : AclPermittedContract
	{
		protected SortedDictionary<Address, Member> addressesToMembers;

		public MemberRegistry(Address address, ContractRegistry registry, Address permissionManager)
			: this(address, registry, permissionManager, null)
		{
		}

		public MemberRegistry(Address address, ContractRegistry registry, Address permissionManager,
			SortedDictionary<Address, Member> addressesToMembers)
			: base(address, registry, permissionManager)
		{
			this.addressesToMembers = addressesToMembers;
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case RegisterMemberAction<Member> registerAction:
					return this.HandleRegisterMember(registerAction);

				case UnregisterMemberAction unregisterAction:
					return this.HandleUnregisterMember(unregisterAction);

				case UpdateMemberAction<Member> updateAction:
					return this.HandleUpdateMember(updateAction);

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
			return this.addressesToMembers[address];
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		private bool HandleRegisterMember(RegisterMemberAction<Member> action)
		{
			Member member = action.Member;
			this.addressesToMembers.Add(member.Address, member);
			return true;
		}

		private bool HandleUnregisterMember(UnregisterMemberAction action)
		{
			return this.addressesToMembers.Remove(action.MemberAddress);
		}

		private bool HandleUpdateMember(UpdateMemberAction<Member> action)
		{
			Member member = action.Member;
			this.addressesToMembers[member.Address] = member;
			return true;
		}
	}
}