using System;
using System.Collections.Generic;
using System.Text;
using ContractsCore;
using TokenSystem.TokenManagerBase;
using WorkTrack;

namespace TaskSystem.Tests
{
	class SplitterPerTaskHoursMock : SplitterPerTaskHours
	{
		public SplitterPerTaskHoursMock(Address address, TokenManager tokenManager, WorkTracker tracker, IList<Address> recipients = null, IDictionary<Address, IDictionary<Address, decimal>> tasksAddresToHours = null)
			: base(address, tokenManager, tracker, recipients, tasksAddresToHours)
		{
		}

		public IDictionary<Address, IDictionary<Address, decimal>> GetTasksAddresToEmployeesHoursPerAddress()
			=> this.TasksAddresToEmployeesHours;
	}
}