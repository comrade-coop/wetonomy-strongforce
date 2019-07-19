using ContractsCore;
using ContractsCore.Actions;

namespace Membership.Actions
{
	public class RegisterMemberAction : Action
	{
		public RegisterMemberAction(string hash, Address target, Member member)
			: base(hash, target)
		{
			this.Member = member;
		}

		public Member Member { get; }
	}
}