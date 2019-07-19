using ContractsCore;
using ContractsCore.Actions;

namespace Achievements
{
	public class CreateAchievementAction : Action
	{
		public CreateAchievementAction(
			string hash,
			Address target,
			Address achiever,
			AchievementMetaData metaData)
			: base(hash, target)
		{
			this.Achiever = achiever;
			this.MetaData = metaData;
		}

		public Address Achiever { get; }

		public AchievementMetaData MetaData { get; }
	}
}