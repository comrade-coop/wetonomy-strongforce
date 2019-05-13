using ContractsCore;
using ContractsCore.Actions;
namespace Members.AddressGroups.Actions
{
	public class ChangeRequiredStakeAction : Action
	{
		public uint RequiredStake;

		public ChangeRequiredStakeAction(string hash, Address target, uint requiredStake) : base(hash, target)
		{
			this.RequiredStake = requiredStake;
		}
	}
}