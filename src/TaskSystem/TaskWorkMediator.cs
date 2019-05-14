using System;
using System.Collections.Generic;
using System.Text;
using TokenSystem.TokenFlow;
using WorkTrack;
using WorkTrack.WorkEventsArgs;

namespace TaskSystem
{
	class TaskWorkMediator
	{
		public TokenSplitter tokenSplitter { get; private set; }

		public delegate int PerformCalculation(int x, int y);

		public TaskWorkMediator(WorkTracker workTracker)
		{
			workTracker.TrackedWork += (_, actionArgs) => this.HandleTrackedWork(actionArgs);
		}

		protected void HandleTrackedWork(WorkEventArgs eventArgs)
		{
			TimeSpan diffResult = eventArgs.Date.Subtract(this.PeriodDate);
			if ((decimal)diffResult.TotalDays >= this.HistoryClearPeriodInDays)
			{
				this.PeriodDate = eventArgs.Date;
				this.EventsHistory = new HashSet<WorkEventArgs>();
			}

			this.EventsHistory.Add(eventArgs);
		}
	}
}