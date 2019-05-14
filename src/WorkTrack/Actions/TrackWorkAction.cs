using System;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace WorkTrack.Actions
{
	public class TrackWorkAction : Action
	{
		public decimal Hours { get; }

		public DateTime Date { get; }

		public Address Employee { get; }

		public Address TaskAddress { get; }

		public TrackWorkAction(string hash, Address target, decimal hours, DateTime date, Address employee, Address taskAddress = null)
			: base(hash, target)
		{
			this.Date = date;
			this.Hours = hours;
			this.Employee = employee;
			this.TaskAddress = taskAddress;
		}
	}
}