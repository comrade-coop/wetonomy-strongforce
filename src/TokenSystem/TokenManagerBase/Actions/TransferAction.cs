// Copyright (c) Comrade Coop. All rights reserved.

using System;
using StrongForce.Core;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TransferAction : PickedTokenAction
	{
		public const string Type = "TransferTokens";

		public const string To = "To";

		private TransferAction()
		{
		}
	}
}