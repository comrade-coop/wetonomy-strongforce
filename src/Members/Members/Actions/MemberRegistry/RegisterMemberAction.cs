using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class RegisterMemberAction<TMemberType> : Action
		where TMemberType : Member
	{
		public TMemberType Member;

		public RegisterMemberAction(string hash, Address target, TMemberType member) : base(hash, target)
		{
			this.Member = member;
		}
	}
}