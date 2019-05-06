using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class UpdateMemberAction : Action
	{
		public Address Member;

		public UpdateMemberAction(string hash, Address target, Address member) : base(hash, target)
		{
			this.Member = member;
		}
	}
}