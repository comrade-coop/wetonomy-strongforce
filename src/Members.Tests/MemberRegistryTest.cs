using ContractsCore;
using TokenSystem.TokenManagerBase;
using Xunit;
using Members.Actions;
using System;

namespace Members.Tests
{
	public class MemberRegistryTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private MemberRegistry memberRegistry;

		public MemberRegistryTest()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.memberRegistry = new MemberRegistry(this.addressFactory.Create(), contractRegistry,
				this.permissionManager.Address);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.memberRegistry);
		}

		[Fact]
		public void AddMember()
		{
			var member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var addMemberAction = new RegisterMemberAction(string.Empty, this.memberRegistry.Address, member);
			this.permissionManager.ExecuteAction(addMemberAction);
			var mem = this.memberRegistry.GetMember(member.Address);
			Assert.Equal(member, mem);
		}

		[Fact]
		public void AddMember_WhenAddresDuplicate_RemainsSingle()
		{
			var member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var addMemberAction = new RegisterMemberAction(string.Empty, this.memberRegistry.Address, member);
			this.permissionManager.ExecuteAction(addMemberAction);
			addMemberAction = new RegisterMemberAction(string.Empty, this.memberRegistry.Address, member);
			Assert.Single(this.memberRegistry.GetAllMembers());
			this.permissionManager.ExecuteAction(addMemberAction);
			Assert.Single(this.memberRegistry.GetAllMembers());
		}

		[Fact]
		public void RemoveMember()
		{
			var member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var addMemberAction = new RegisterMemberAction(string.Empty, this.memberRegistry.Address, member);
			this.permissionManager.ExecuteAction(addMemberAction);
			var removeMemberAction = new UnregisterMemberAction(string.Empty, this.memberRegistry.Address, member.Address);
			this.permissionManager.ExecuteAction(removeMemberAction);
			var mem = this.memberRegistry.GetMember(member.Address);
			Assert.Null(mem);
		}
	}
}