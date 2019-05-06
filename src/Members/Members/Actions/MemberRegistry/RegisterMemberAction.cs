using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class RegisterMemberAction : Action
	{
		public Member Member;

		public RegisterMemberAction(string hash, Address target, Member member) : base(hash, target)
		{
			this.Member = member;
		}
	}
}