using System;
using System.Collections.Generic;
using System.Text;

namespace TaskSystem.Exceptions
{
	class TaskNotInRequiredStageException : Exception
	{
		public TaskNotInRequiredStageException(Task task, Type action)
			: base($"Task {task.Id}:\"{task.Header}\" not in required stage to perform: \"{action.ToString()}\". Current stage: \"{task.Stage}\"")
		{
		}

		public TaskNotInRequiredStageException(Task task, string action)
			: base($"Task {task.Id}:\"{task.Header}\" not in required Stage to perform: {action}. Current stage: \"{task.Stage}\"")
		{
		}
	}
}