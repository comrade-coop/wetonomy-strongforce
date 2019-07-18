using ContractsCore;
using Membership.AddressGroups;
using Membership.AddressGroups.Actions;
using TokenSystem.TokenManagerBase;
using Xunit;

namespace Membership.Tests
{
	public class AddressGroupRegistryTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private readonly AddressGroupRegistry addressGroupRegistry;

		public AddressGroupRegistryTest()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.addressGroupRegistry = new AddressGroupRegistry(
				this.addressFactory.Create(),
				this.contractRegistry,
				this.permissionManager.Address,
				null);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.addressGroupRegistry);
		}

		[Fact]
		public void AddGroup()
		{
			var group = new AddressGroup(
				this.addressFactory.Create(),
				this.contractRegistry,
				this.permissionManager.Address);
			var registerAction = new RegisterGroupAction(string.Empty, this.addressGroupRegistry.Address, group);
			this.permissionManager.ExecuteAction(registerAction);

			Assert.True(this.addressGroupRegistry.GetAllGroups().Contains(group));
		}

		[Fact]
		public void AddGroup_WhenAddressDuplicate_RemainsSingle()
		{
			var group = new AddressGroup(
				this.addressFactory.Create(),
				this.contractRegistry,
				this.permissionManager.Address);
			var addMemberAction = new RegisterGroupAction(string.Empty, this.addressGroupRegistry.Address, group);
			this.permissionManager.ExecuteAction(addMemberAction);
			Assert.Single(this.addressGroupRegistry.GetAllGroups());
			addMemberAction = new RegisterGroupAction(string.Empty, this.addressGroupRegistry.Address, group);
			this.permissionManager.ExecuteAction(addMemberAction);
			Assert.Single(this.addressGroupRegistry.GetAllGroups());
		}

		[Fact]
		public void RemoveGroup()
		{
			var group = new AddressGroup(
				this.addressFactory.Create(),
				this.contractRegistry,
				this.permissionManager.Address);
			var addMemberAction = new RegisterGroupAction(
				string.Empty,
				this.addressGroupRegistry.Address,
				group);
			this.permissionManager.ExecuteAction(addMemberAction);
			var removeMemberAction =
				new UnregisterGroupAction(string.Empty, this.addressGroupRegistry.Address, group.Address);
			this.permissionManager.ExecuteAction(removeMemberAction);

			Assert.False(this.addressGroupRegistry.GetAllGroups().Contains(group));
		}
	}
}