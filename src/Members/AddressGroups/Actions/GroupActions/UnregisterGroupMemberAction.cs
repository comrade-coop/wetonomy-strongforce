using ContractsCore;
using ContractsCore.Actions;

namespace Members.AddressGroups.Actions
{
	public class UnregisterGroupMemberAction : Action
	{
		public Address MemberAddress;

		public UnregisterGroupMemberAction(string hash, Address target, Address member) : base(hash, target)
		{
			this.MemberAddress = member;
		}
	}
}