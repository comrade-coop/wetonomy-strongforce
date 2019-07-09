using System;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace WorkTrack.Actions
{
	public class TrackWorkAction : Action
	{
		public decimal Hours { get; set; }

		public DateTime Date { get; set; }

		public Address Employee { get; private set; }

		public TrackWorkAction(string hash, Address target, decimal hours, DateTime date, Address employee)
			: base(hash, target)
		{
			this.Date = date;
			this.Hours = hours;
			this.Employee = employee;
		}
	}
}