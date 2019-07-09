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
			return (this.value as string).CompareTo(obj.GetValue() as string);
		}

		public override object GetValue()
		{
			return this.value.ToString();
		}

		public override bool SetValue(object value)
		{
			this.value = value;
			return true;
		}
	}
}