using System;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem.Actions
{
	public class ChangeDefaultSplitterAction : Action
	{
		public Address Splitter { get; }

		public ChangeDefaultSplitterAction(string hash, Address target, Address splitter)
			: base(hash, target)
		{
			this.Splitter = splitter ?? throw new NullReferenceException();
		}
	}
}