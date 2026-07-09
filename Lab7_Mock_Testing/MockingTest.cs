using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using ScriptsToTest.Mock;

namespace Mock
{
    public class MockingTest
    {
        [Test]
        public async Task PurchaseItemAsync_Success_ExactOrderExecutionTest()
        {
            var wallet = Substitute.For<IWallet>();
            var inventory = Substitute.For<IInventory>();
            var analytics = Substitute.For<IAnalytics>();
            var backend = Substitute.For<IBackendValidator>();
            
            var shop = new ShopManager(analytics, backend, inventory, wallet);
            
            wallet.HasEnoughMoney(100).Returns(true);
            inventory.TryAddItem("+2_longsword").Returns(true);
            backend.ValidateReceiptAsync("+2_longsword").Returns(Task.FromResult(true));
            
            bool result = await shop.PurchaseItemAsync("+2_longsword", 100);
            
            Assert.IsTrue(result);
            
            Received.InOrder(() =>
            {
                wallet.Withdraw(100);
                backend.ValidateReceiptAsync("+2_longsword");
                inventory.TryAddItem("+2_longsword");
                analytics.LogTransaction("+2_longsword", true);
            });
        }

        [Test]
        public async Task PurchaseItemAsync_Rollback_BackendExceptionThrow()
        {
            var wallet = Substitute.For<IWallet>();
            var inventory = Substitute.For<IInventory>();
            var analytics = Substitute.For<IAnalytics>();
            var backend = Substitute.For<IBackendValidator>();
            
            var shop = new ShopManager(analytics, backend, inventory, wallet);
            
            wallet.HasEnoughMoney(50).Returns(true);
            backend.ValidateReceiptAsync("greater_healing_potion").ThrowsAsync(new TimeoutException("No connection"));
            
            bool result = await shop.PurchaseItemAsync("greater_healing_potion", 50);
            
            Assert.IsFalse(result);
            
            wallet.Received().Deposit(50);
            wallet.Received().Withdraw(50);
            inventory.DidNotReceive().TryAddItem(Arg.Any<string>());
        }

        [Test]
        public async Task PurchaseItemAsync_ReturnCorrect_IDBasedValidation()
        {
            var wallet = Substitute.For<IWallet>();
            var inventory = Substitute.For<IInventory>();
            var analytics = Substitute.For<IAnalytics>();
            var backend = Substitute.For<IBackendValidator>();
            
            var shop = new ShopManager(analytics, backend, inventory, wallet);
            
            wallet.HasEnoughMoney(Arg.Any<int>()).Returns(true);
            inventory.TryAddItem(Arg.Any<string>()).Returns(true);
            backend.ValidateReceiptAsync(Arg.Any<string>())
                .Returns(x => Task.FromResult(x.ArgAt<string>(0).StartsWith("validated_")));
            
            bool success = await shop.PurchaseItemAsync("validated_platinum_pendant", 10000);
            bool fail = await shop.PurchaseItemAsync("diamond_pendant", 19899);
            
            Assert.IsTrue(success);
            Assert.IsFalse(fail);
        }
        
        [Test]
        public void Wallet_WhenBalanceChanges_TriggersEvent()
        {
            var wallet = Substitute.For<IWallet>();
    
            bool eventFired = false;
            int updatedBalance = 0;
            
            wallet.OnBalanceChanged += (newBalance) => 
            {
                eventFired = true;
                updatedBalance = newBalance;
            };


            wallet.OnBalanceChanged += Raise.Event<Action<int>>(500);
            
            Assert.IsTrue(eventFired);
            Assert.AreEqual(500, updatedBalance);
        }
        
        [Test]
        public void TryGetItemPrice_OutParameters()
        {
            var inventory = Substitute.For<IInventory>();
            
            inventory.TryGetItemPrice("epic_sword", out Arg.Any<int>())
                .Returns(x => 
                {
                    x[1] = 250;  
                    return true; 
                });
            
            bool hasPrice = inventory.TryGetItemPrice("epic_sword", out int actualPrice);
            
            Assert.IsTrue(hasPrice);
            Assert.AreEqual(250, actualPrice);
        }

        [Test]
        public void PurchaseMultipleItems_ExactAmountOfMoneyWithdrawn()
        {
            var wallet = Substitute.For<IWallet>();

            for (int i = 0; i < 3; i++)
            {
                wallet.Withdraw(25);
            }
            
            wallet.Received(3).Withdraw(25);
        }
    }
}