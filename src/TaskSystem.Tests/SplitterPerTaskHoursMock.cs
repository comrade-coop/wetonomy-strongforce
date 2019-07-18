using System.Collections.Generic;
using ContractsCore;
using TokenSystem.TokenManagerBase;

namespace TaskSystem.Tests
{
	class SplitterPerTaskHoursMock : SplitterPerTaskHours
	{
		public SplitterPerTaskHoursMock(
			Address address,
			Address tokenManager,
			WorkTracker.WorkTracker tracker,
			ISet<Address> recipients = null,
			IDictionary<Address, IDictionary<Address, decimal>> tasksAddressToHours = null)
			: base(address, tokenManager, tracker, recipients, tasksAddressToHours)
		{
		}

		public IDictionary<Address, IDictionary<Address, decimal>> GetTasksAddresToEmployeesHoursPerAddress()
			=> this.TasksAddresToEmployeesHours;
	}
}