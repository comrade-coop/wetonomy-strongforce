using System;
using ContractsCore;

namespace Membership.Exceptions
{
	public class NotRegisteredReceivedStrategyException : Exception
	{
		public NotRegisteredReceivedStrategyException(ContractsCore.Actions.Action action, Address memberAddress)
			: base($"There is no ReceivedStrategy registered for \"{action.Sender}\" in {memberAddress}\" ")
		{
		}
	}
}