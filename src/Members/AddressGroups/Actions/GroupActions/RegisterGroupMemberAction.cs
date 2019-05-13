using ContractsCore;
using ContractsCore.Actions;

namespace Members.AddressGroups.Actions
{
	public class RegisterGroupMemberAction : Action
	{
		public Address MemberAddress;

		public RegisterGroupMemberAction(string hash, Address target, Address member) : base(hash, target)
		{
			this.MemberAddress = member;
		}
	}
}