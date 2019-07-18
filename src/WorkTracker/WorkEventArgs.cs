using System;
using ContractsCore;

namespace WorkTracker
{
	public class WorkEventArgs : EventArgs, IComparable<WorkEventArgs>
	{
		public WorkEventArgs(decimal hours, DateTime date, Address employee, Address taskAddress = null)
		{
			this.Date = date;
			this.Hours = hours;
			this.Employee = employee ?? throw new ArgumentNullException();
			this.TaskAddress = taskAddress;
		}

		public decimal Hours { get; }

		public DateTime Date { get; }

		public Address Employee { get; }

		public Address TaskAddress { get; }

		public int CompareTo(WorkEventArgs other)
		{
			int comparison = this.Employee.CompareTo(other.Employee);
			if (comparison != 0)
			{
				return comparison;
			}

			return this.Hours == other.Hours ? this.Date.CompareTo(other.Date) : this.Hours.CompareTo(other.Hours);
		}
	}
}