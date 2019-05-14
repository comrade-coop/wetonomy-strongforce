using System;
using System.Collections.Generic;
using System.Text;

namespace TaskSystem.Exceptions
{
	public class TaskUnassignedException : Exception
	{
		public TaskUnassignedException(Task task)
			: base($"Task {task.Id}:\"{task.Header}\" does not have assignee")
		{
		}
	}
}