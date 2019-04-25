using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensMintedAction<TTokenTagType> : Action
	{
		public TokensMintedAction(
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