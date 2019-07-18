using System;
using System.Collections.Generic;
using System.Text;
using ContractsCore;
using TokenSystem.TokenFlow;
using WorkTrack;

namespace TaskSystem
{
	public class TaskWorkMediator
	{
		public TokenSplitter tokenSplitter { get; }

		public delegate bool TrackWorkHours(Address employeeAddress, decimal amout, Address taskAddress);

		private TrackWorkHours trackCallback;

		public TaskWorkMediator(WorkTracker workTracker, TrackWorkHours callback)
		{
			workTracker.TrackedWork += (_, actionArgs) => this.HandleTrackedWork(actionArgs);
			this.trackCallback = callback;
		}

		protected void HandleTrackedWork(WorkEventArgs eventArgs)
		{
			this.trackCallback(eventArgs.Employee, eventArgs.Hours, eventArgs.TaskAddress);
		}
	}
}