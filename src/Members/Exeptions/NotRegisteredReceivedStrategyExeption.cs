using System;
using System.Collections.Generic;
using System.Text;
using ContractsCore;

namespace Members.Exeptions
{
	class NotRegisteredReceivedStrategyExeption: Exception
	{
		public NotRegisteredReceivedStrategyExeption(ContractsCore.Actions.Action action, Address memberAddress)
			: base($"There is no ReceivedStrategy registered for \"{action.Sender}\" in {memberAddress}\" ")
		{
		}
	}
}