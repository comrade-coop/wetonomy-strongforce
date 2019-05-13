using System;
using System.Collections.Generic;
using System.Text;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.TokenTags
{
	public class StringTag : TokenTagBase
	{
		public StringTag(string value)
			: base(typeof(string))
		{
			this.value = value;
		}

		public override int CompareTo(TokenTagBase obj)
		{
			return (this.value as string).CompareTo(obj.Value() as string);
		}

		public override object Value()
		{
			return this.value.ToString();
		}

		public override bool Value(object value)
		{
			this.value = value;
			return true;
		}
	}
}