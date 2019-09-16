// Copyright (c) Comrade Coop. All rights reserved.

using System;
using StrongForce.Core;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class BurnOtherAction : PickedTokenAction
	{
		public const string Type = "BurnOtherTokens";

		public const string From = "From";

		private BurnOtherAction()
		{
		}
	}
}