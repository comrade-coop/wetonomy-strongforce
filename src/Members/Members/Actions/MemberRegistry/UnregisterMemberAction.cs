using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class UnregisterMemberAction : Action
	{
		public Address MemberAddress;

		public UnregisterMemberAction(string hash, Address target, Address member) : base(hash, target)
		{
			this.MemberAddress = member;
		}
	}
}