using System;
using ContractsCore;

namespace TaskSystem.Exceptions
{
	public class UnallowedRewardReceiverException: ArgumentException
	{
		public UnallowedRewardReceiverException(TokenRelay relay, Address rewardReceiver)
			: base($"TokenRelay {relay.Address.ToString()}: cant have Reward Receiver \"{rewardReceiver.ToString()}\"")
		{
		}
	}
}