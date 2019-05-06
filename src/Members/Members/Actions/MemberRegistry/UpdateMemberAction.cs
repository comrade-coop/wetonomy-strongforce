using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class UpdateMemberAction<TMemberType> : Action
		where TMemberType : Member
	{
		public TMemberType Member;

		public UpdateMemberAction(string hash, Address target, TMemberType member) : base(hash, target)
		{
			this.Member = member;
		}
	}
}