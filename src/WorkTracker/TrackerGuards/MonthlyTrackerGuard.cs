using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkTracker.Actions;

namespace WorkTracker.TrackerGuards
{
	public class MonthlyTrackerGuard : TrackerGuardBase
	{
		public decimal HoursPerMonth { get; set; }

		public MonthlyTrackerGuard(WorkEventLog eventLog, decimal hoursPerMonth = 176)
			: base("MonthlyTrackerGuardBase", eventLog)
		{
			this.HoursPerMonth = hoursPerMonth;
		}

		public override bool Validate(TrackWorkAction action)
		{
			DateTime today = action.Date;
			int month = today.Month;
			decimal hoursSum = this.EventLog.EventsHistory
				.Where(workEvent => (workEvent.Date.Month == month) &&
					workEvent.Employee.Equals(action.Employee))
				.Select(args => args.Hours).Sum();
			return hoursSum + action.Hours < this.HoursPerMonth;
		}
	}
}