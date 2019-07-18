using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ContractsCore;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem
{
	public class SplitterPerTaskHours : TokenSplitter
	{
		protected IDictionary<Address, IDictionary<Address, decimal>> TasksAddresToEmployeesHours { get; set; }

		public SplitterPerTaskHours(
			Address address,
			Address tokenManager,
			WorkTracker.WorkTracker tracker,
			ISet<Address> recipients = null,
			IDictionary<Address, IDictionary<Address, decimal>> tasksAddressToHours = null)
			: base(address, tokenManager, recipients)
		{
			WorkTracker.WorkTracker workTracker = tracker ?? throw new NullReferenceException();
			//WTF????
			new TaskWorkMediator(workTracker, this.TrackWorkHours);
			this.TasksAddresToEmployeesHours =
				tasksAddressToHours ?? new SortedDictionary<Address, IDictionary<Address, decimal>>();
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
				var emplooyees = new SortedDictionary<Address, decimal>() {{employeeAddress, amout}};
				this.TasksAddresToEmployeesHours.Add(taskAddress, emplooyees);
			}

			return true;
		}

		protected bool HandleTokensReceived(TokensReceivedAction action)
		{
			IDictionary<Address, decimal> employeesToHours = this.TasksAddresToEmployeesHours
				.FirstOrDefault(task => task.Key == action.TokensSender).Value;
			this.Split(action.Tokens, employeesToHours);
			return true;
		}

		protected override void Split(IReadOnlyTaggedTokens receivedTokens, object recipients)
		{
			var employeesToHours = recipients as SortedDictionary<Address, decimal>;
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
					this.Address,
					employee);
				this.OnSend(transferAction);
			}
		}
	}
}