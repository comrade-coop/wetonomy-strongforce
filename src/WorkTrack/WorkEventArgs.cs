using System;
using ContractsCore;

namespace WorkTrack
{
	public class WorkEventArgs: EventArgs, IComparable<WorkEventArgs>
	{
		public decimal Hours { get; }

		public DateTime Date { get; }

		public Address Employee { get; }

		public Address TaskAddress { get; }

		public WorkEventArgs(decimal hours, DateTime date, Address employee, Address taskAddress = null)
		{
			this.Date = date;
			this.Hours = hours;
			this.Employee = employee ?? throw new NullReferenceException();
			this.TaskAddress = taskAddress;
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