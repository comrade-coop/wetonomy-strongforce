using System;
using System.Linq;
using WorkTracker.Actions;

namespace WorkTracker.TrackerGuards
{
	public class WeeklyTrackerGuard : TrackerGuardBase
	{
		public decimal HoursPerWeek { get; set; }

		public WeeklyTrackerGuard(WorkEventLog eventLog, decimal hoursPerWeek = 40)
			: base("WeeklyTracketGuard", eventLog)
		{
			this.HoursPerWeek = hoursPerWeek;
		}

		public override bool Validate(TrackWorkAction action)
		{
			DateTime today = action.Date;
			int dayOfWeek = (int)today.DayOfWeek;
			decimal hoursSum = this.EventLog.EventsHistory
				.Where(workEvent =>
					(today.DayOfYear - workEvent.Date.DayOfYear <= dayOfWeek) &&
					workEvent.Employee.Equals(action.Employee))
				.Select(args => args.Hours).Sum();
			return hoursSum + action.Hours < this.HoursPerWeek;
		}
	}
}