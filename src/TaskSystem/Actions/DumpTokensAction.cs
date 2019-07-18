using ContractsCore;
using ContractsCore.Actions;

namespace TaskSystem.Actions
{
	public class DumpTokensAction : Action
	{
		public DumpTokensAction(string hash, Address target)
			: base(hash, target)
		{
		}
	}
}