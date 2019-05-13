using ContractsCore;
using ContractsCore.Actions;

namespace Members.AddressGroups.Actions
{
	public class RegisterGroupAction : Action
	{
		public AddressGroup AddressGroup;

		public RegisterGroupAction(string hash, Address target, AddressGroup addressGroup) : base(hash, target)
		{
			this.AddressGroup = addressGroup;
		}
	}
}