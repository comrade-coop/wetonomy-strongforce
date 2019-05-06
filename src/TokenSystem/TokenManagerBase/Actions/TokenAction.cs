using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TokenAction : Action
	{
		public TokenAction(
			string hash,
			Address target,
			IReadOnlyTaggedTokens tokens)
			: base(hash, target)
		{
			this.Tokens = tokens;
		}

		public IReadOnlyTaggedTokens Tokens { get; }
	}
}