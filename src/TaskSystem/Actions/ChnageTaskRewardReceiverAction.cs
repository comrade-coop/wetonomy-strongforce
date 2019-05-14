using ContractsCore;
using ContractsCore.Actions;

namespace TaskSystem.Actions
{
	public class ChnageTaskRewardReceiverAction : Action
	{
		public Address RewardReceiver { get; }

		public ChnageTaskRewardReceiverAction(string hash, Address target, Address rewardReceiver)
			: base(hash, target)
		{
			this.RewardReceiver = rewardReceiver;
		}
	}
}