using ContractsCore;
using ContractsCore.Actions;

namespace TaskSystem.Actions
{
	public class ChangeStageAction : Action
	{
		public ChangeStageAction(string hash, Address target, TaskStage stage) : base(hash, target)
		{
			this.Stage = stage;
		}

		public TaskStage Stage { get; }
	}
}