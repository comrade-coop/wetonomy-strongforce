using ContractsCore;
using ContractsCore.Actions;

namespace Membership.Actions
{
	public class UnregisterMemberAction : Action
	{
		public UnregisterMemberAction(string hash, Address target, Address member)
			: base(hash, target)
		{
			this.MemberAddress = member;
		}

		public Address MemberAddress { get; }
	}
}