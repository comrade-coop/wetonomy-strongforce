using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ContractsCore;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem
{
	public class SplitterPerTaskHours : TokenSplitter
	{
		protected IDictionary<Address, IDictionary<Address, decimal>> TasksAddressToEmployeesHours { get; }

		public SplitterPerTaskHours(
			Address address,
			Address tokenManager,
			WorkTracker.WorkTracker tracker,
			ISet<Address> recipients = null,
			IDictionary<Address, IDictionary<Address, decimal>> tasksAddressToHours = null)
			: base(address, tokenManager, recipients)
		{
			new TaskWorkMediator(tracker, this.TrackWorkHours);
			this.TasksAddressToEmployeesHours =
				tasksAddressToHours ?? new SortedDictionary<Address, IDictionary<Address, decimal>>();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction tokenAction:
					return this.HandleTokensReceived(tokenAction);

				default:
					return false;
			}
		}

		protected override void Split(IReadOnlyTaggedTokens receivedTokens, object recipients)
		{
			var employeesToHours = (SortedDictionary<Address, decimal>) recipients;
			decimal hours = employeesToHours.Sum(x => x.Value);
			BigInteger splitBase = receivedTokens.TotalBalance / (BigInteger) hours;

			if (splitBase <= 0)
			{
				return;
			}

			foreach ((Address employee, var employeeHours) in employeesToHours)
			{
				BigInteger amount = splitBase * (BigInteger) employeeHours;
				var transferAction = new TransferAction(
					string.Empty,
					this.TokenManager,
					amount,
					employee);
				this.OnSend(transferAction);
			}
		}

		private bool TrackWorkHours(Address employeeAddress, decimal amount, Address taskAddress)
		{
			if (this.TasksAddressToEmployeesHours.ContainsKey(taskAddress))
			{
				if (this.TasksAddressToEmployeesHours[taskAddress].ContainsKey(employeeAddress))
				{
					this.TasksAddressToEmployeesHours[taskAddress][employeeAddress] += amount;
				}
				else
				{
					this.TasksAddressToEmployeesHours[taskAddress].Add(employeeAddress, amount);
				}
			}
			else
			{
				var employees = new SortedDictionary<Address, decimal>() {{employeeAddress, amount}};
				this.TasksAddressToEmployeesHours.Add(taskAddress, employees);
			}

			return true;
		}

		private bool HandleTokensReceived(TokensReceivedAction action)
		{
			IDictionary<Address, decimal> employeesToHours = this.TasksAddressToEmployeesHours
				.FirstOrDefault(task => Equals(task.Key, action.TokensSender)).Value;
			this.Split(action.Tokens, employeesToHours);
			return true;
		}
	}
}