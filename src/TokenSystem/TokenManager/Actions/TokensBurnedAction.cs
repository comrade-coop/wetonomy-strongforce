using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensBurnedAction<TTokenTagType> : Action
	{
		public TokensBurnedAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, origin, sender, target)
		{
			this.Tokens = tokens;
		}

		public IReadOnlyTaggedTokens<TTokenTagType> Tokens { get; }
	}
}