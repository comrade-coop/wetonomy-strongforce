namespace TokenSystem.Tokens
{
	public interface ITokenPicker<TTagType>
	{
		TaggedTokens<TTagType> Pick(TaggedTokens<TTagType> tokens, decimal amount);
	}
}