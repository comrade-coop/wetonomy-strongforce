using ContractsCore;
using ContractsCore.Actions;

namespace Membership.AddressGroups.Actions
{
	public class UnregisterGroupAction : Action
	{
		public Address AddressGroup;

		public UnregisterGroupAction(string hash, Address target, Address addressGroup) : base(hash, target)
		{
			this.AddressGroup = addressGroup;
		}
	}
}