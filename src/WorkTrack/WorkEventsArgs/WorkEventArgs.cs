using System;
using ContractsCore;

namespace WorkTrack.WorkEventsArgs
{
	public class WorkEventArgs: EventArgs, IComparable<WorkEventArgs>
	{
		public decimal Hours { get; set; }

		public DateTime Date { get; set; }

		public Address Employee { get; set; }

		public WorkEventArgs(decimal hours, DateTime date, Address employee)
		{
			this.Date = date;
			this.Hours = hours;
			this.Employee = employee ?? throw new NullReferenceException();
		}

		public int CompareTo(WorkEventArgs other)
		{
			int comparison = this.Employee.CompareTo(other.Employee);
			if (comparison != 0)
			{
				return comparison;
			}
			else if (this.Hours == other.Hours)
			{
				return this.Date.CompareTo(other.Date);
			}
			else
			{
				return this.Hours.CompareTo(other.Hours);
			}
		}
	}
}