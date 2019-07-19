using ContractsCore;
using ContractsCore.Actions;

namespace Membership.Actions
{
	public class UpdateMemberAction : Action
	{
		public UpdateMemberAction(string hash, Address target, Address member)
			: base(hash, target)
		{
			this.Member = member;
		}

		public Address Member { get; }
	}
}