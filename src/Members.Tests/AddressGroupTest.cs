using ContractsCore;
using Members.AddressGroups;
using Members.AddressGroups.Actions;
using TokenSystem.TokenManagerBase;
using Xunit;

namespace Members.Tests
{
	public class AddressGroupTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private AddressGroup addressGroup;

		public AddressGroupTest()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.addressGroup = new AddressGroup(this.addressFactory.Create(), this.contractRegistry,
				this.permissionManager.Address);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.addressGroup);
		}

		[Fact]
		public void AddMember()
		{
			var address = this.addressFactory.Create();
			var addMember = new RegisterGroupMemberAction(string.Empty, this.addressGroup.Address, address);
			this.permissionManager.ExecuteAction(addMember);
			Assert.True(this.addressGroup.IsMember(address));
		}

		[Fact]
		public void AddMember_WhenAddresDuplicate_RemainsSingle()
		{
			var address = this.addressFactory.Create();
			var addMember = new RegisterGroupMemberAction(string.Empty, this.addressGroup.Address, address);
			this.permissionManager.ExecuteAction(addMember);
			Assert.Single(this.addressGroup.GetAllMembers());
			addMember = new RegisterGroupMemberAction(string.Empty, this.addressGroup.Address, address);
			this.permissionManager.ExecuteAction(addMember);
			Assert.Single(this.addressGroup.GetAllMembers());
		}

		[Fact]
		public void RemoveMember()
		{
			var address = this.addressFactory.Create();
			var addMember = new RegisterGroupMemberAction(string.Empty, this.addressGroup.Address, address);
			this.permissionManager.ExecuteAction(addMember);
			var removeMemberAction = new UnregisterGroupMemberAction(string.Empty, this.addressGroup.Address, address);
			this.permissionManager.ExecuteAction(removeMemberAction);
			Assert.False(this.addressGroup.IsMember(address));
		}
	}
}