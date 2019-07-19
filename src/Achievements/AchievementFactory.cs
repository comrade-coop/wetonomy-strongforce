using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using ContractsCore.Permissions;

namespace Achievements
{
	public class AchievementFactory : AclPermittedContract
	{
		public AchievementFactory(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			Address burnTokenManager,
			Address mintTokenManager,
			int exchangeRate,
			Address achievementsGroup)
			: this(
				address,
				registry,
				permissionManager,
				new AccessControlList(),
				burnTokenManager,
				mintTokenManager,
				exchangeRate,
				achievementsGroup)
		{
		}

		public AchievementFactory(
			Address address,
			ContractRegistry registry,
			Address permissionManager,
			AccessControlList acl,
			Address burnTokenManager,
			Address mintTokenManager,
			int exchangeRate,
			Address achievementsGroup)
			: base(address, registry, permissionManager, acl)
		{
			this.BurnTokenManager = burnTokenManager;
			this.MintTokenManager = mintTokenManager;
			this.ExchangeRate = exchangeRate;
		}

		public Address BurnTokenManager { get; }

		public Address MintTokenManager { get; }

		public int ExchangeRate { get; }


		protected override object GetState()
		{
			throw new System.NotImplementedException();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case CreateAchievementAction achievementAction:
					return this.CreateAchievement(achievementAction);
				default:
					return false;
			}
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new System.NotImplementedException();
		}

		private bool CreateAchievement(CreateAchievementAction achievementAction)
		{
			// TODO: Implement Achievement creation, and adding to Achievements group
			var addressFactory = new RandomAddressFactory();
			var achievement = new Achievement(
				addressFactory.Create(),
				this.BurnTokenManager,
				this.MintTokenManager,
				this.ExchangeRate,
				achievementAction.Achiever,
				achievementAction.MetaData);
			this.registry.RegisterContract(achievement);

			//TODO: Should add the achievement to the achievements group

			return true;
		}
	}
}