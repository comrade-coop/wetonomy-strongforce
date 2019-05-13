using System;
using System.Collections.Generic;
using System.Text;

namespace TokenSystem.Tokens
{
	public abstract class TokenTagBase : IComparable<TokenTagBase>
	{
		protected readonly Type tagType;

		protected object value;

		public TokenTagBase(Type tagType)
		{
			this.tagType = tagType;
		}

		public abstract int CompareTo(TokenTagBase obj);

		public Type GetTagType()
		{
			return this.tagType;
		}

		public abstract object Value();

		public abstract bool Value(object value);
	}
}