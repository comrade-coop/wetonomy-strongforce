using ContractsCore;
using TokenSystem.TokenManagerBase;
using Xunit;
using System;
using Membership.Actions;

namespace Membership.Tests
{
	public class MemberRegistryTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private MembersRegistry membersRegistry;

		public MemberRegistryTest()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.membersRegistry = new MembersRegistry(this.addressFactory.Create(), contractRegistry,
				this.permissionManager.Address);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.membersRegistry);
		}

		[Fact]
		public void AddMember()
		{
			var member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var addMemberAction = new RegisterMemberAction(string.Empty, this.membersRegistry.Address, member);
			this.permissionManager.ExecuteAction(addMemberAction);
			var mem = this.membersRegistry.GetMember(member.Address);
			Assert.Equal(member, mem);
		}

		[Fact]
		public void AddMember_WhenAddresDuplicate_RemainsSingle()
		{
			var member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var addMemberAction = new RegisterMemberAction(string.Empty, this.membersRegistry.Address, member);
			this.permissionManager.ExecuteAction(addMemberAction);
			addMemberAction = new RegisterMemberAction(string.Empty, this.membersRegistry.Address, member);
			Assert.Single(this.membersRegistry.GetAllMembers());
			this.permissionManager.ExecuteAction(addMemberAction);
			Assert.Single(this.membersRegistry.GetAllMembers());
		}

		[Fact]
		public void RemoveMember()
		{
			var member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var addMemberAction = new RegisterMemberAction(string.Empty, this.membersRegistry.Address, member);
			this.permissionManager.ExecuteAction(addMemberAction);
			var removeMemberAction = new UnregisterMemberAction(string.Empty, this.membersRegistry.Address, member.Address);
			this.permissionManager.ExecuteAction(removeMemberAction);
			var mem = this.membersRegistry.GetMember(member.Address);
			Assert.Null(mem);
		}
	}
}