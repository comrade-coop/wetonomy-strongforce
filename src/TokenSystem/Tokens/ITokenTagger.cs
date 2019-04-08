using ContractsCore;

namespace TokenSystem.Tokens
{
	public interface ITokenTagger
	{
		TaggedTokens Tag(Address owner, decimal amount);
	}
}