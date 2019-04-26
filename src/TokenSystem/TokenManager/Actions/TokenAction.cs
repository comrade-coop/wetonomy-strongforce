using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokenAction<TTokenTagType> : Action
	{
		public TokenAction(
			string hash,
			Address target,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, target)
		{
			this.Tokens = tokens;
		}

		public IReadOnlyTaggedTokens<TTokenTagType> Tokens { get; }
	}
}