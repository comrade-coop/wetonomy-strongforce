using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using Action = ContractsCore.Actions.Action;

namespace WorkTracker
{
	public class WorkTrackerController : AclPermittedContract
	{
		protected WorkEventLog eventLog;

		protected WorkTracker workTracker;

		public WorkTrackerController(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			WorkEventLog eventLog,
			List<TrackerGuardBase> trackGuards,
			WorkTracker workTracker)
			: base(address, registry, permissionManager)
		{
			this.workTracker = workTracker ??
			                   new WorkTracker(null, null, this.Address);
			this.eventLog = eventLog ??
			                new WorkEventLog(30, new HashSet<WorkEventArgs>(), this.workTracker);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			throw new NotImplementedException();
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}
	}
}