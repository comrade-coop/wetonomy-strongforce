using ContractsCore;
using ContractsCore.Actions;

namespace Membership.AddressGroups.Actions
{
	public class UpdateGroupAction : Action
	{
		public AddressGroup AddressGroup;

		public UpdateGroupAction(string hash, Address target, AddressGroup addressGroup)
			: base(hash, target)
		{
			this.AddressGroup = addressGroup;
		}
	}
}