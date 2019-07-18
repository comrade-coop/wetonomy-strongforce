using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ContractsCore;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using WorkTrack;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem
{
	public class SplitterPerTaskHours : TokenSplitter
	{
		protected IDictionary<Address, IDictionary<Address, decimal>> TasksAddresToEmployeesHours { get; set; }

		public SplitterPerTaskHours(
			Address address,
			TokenManager tokenManager,
			WorkTracker tracker,
			IList<Address> recipients = null,
			IDictionary<Address, IDictionary<Address, decimal>> tasksAddresToHours = null)
			: base(address, tokenManager, recipients)
		{
			var workTracker = tracker ?? throw new NullReferenceException();
			new TaskWorkMediator(workTracker, this.TrackWorkHours);
			this.TasksAddresToEmployeesHours = tasksAddresToHours ?? new SortedDictionary<Address, IDictionary<Address, decimal>>();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction tokenAction:
					return this.HandleTokensReceived(tokenAction);

				default: return false;
			}
		}

		protected bool TrackWorkHours(Address employeeAddress, decimal amout, Address taskAddress)
		{
			if (this.TasksAddresToEmployeesHours.ContainsKey(taskAddress))
			{
				if (this.TasksAddresToEmployeesHours[taskAddress].ContainsKey(employeeAddress))
				{
					this.TasksAddresToEmployeesHours[taskAddress][employeeAddress] += amout;
				}
				else
				{
					this.TasksAddresToEmployeesHours[taskAddress].Add(employeeAddress, amout);
				}
			}
			else
			{
				var emplooyees = new SortedDictionary<Address, decimal>() { { employeeAddress, amout } };
				this.TasksAddresToEmployeesHours.Add(taskAddress, emplooyees);
			}

			return true;
		}

		protected bool HandleTokensReceived(TokensReceivedAction action)
		{
			IDictionary<Address, decimal> employeesToHours = this.TasksAddresToEmployeesHours.FirstOrDefault(task => task.Key == action.TokensSender).Value;
			this.Split(action.Tokens, employeesToHours);
			return true;
		}

		protected override void Split(IReadOnlyTaggedTokens receivedTokens, object recipients)
		{
			var employeesToHours = recipients as SortedDictionary<Address, decimal>;
			decimal hours = employeesToHours.Sum(x => x.Value);
			BigInteger splitBase = receivedTokens.TotalTokens / (BigInteger)hours;

			if (splitBase <= 0)
			{
				return;
			}

			foreach (var employee in employeesToHours)
			{
				var amount = splitBase * (BigInteger)employee.Value;
				var transferAction = new TransferAction(
					string.Empty,
					this.TokenManager.Address,
					amount,
					this.Address,
					employee.Key);
				this.OnSend(transferAction);
			}
		}
	}
}