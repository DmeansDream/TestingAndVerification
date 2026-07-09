using System;
using System.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;

namespace ScriptsToTest.Mock
{
    public class ShopManager
    {
        private readonly IAnalytics _analytics;
        private readonly IBackendValidator _backendValidator;
        private readonly IInventory _inventory;
        private readonly IWallet _wallet;

        public ShopManager(IAnalytics analytics, 
            IBackendValidator backendValidator, 
            IInventory inventory,
            IWallet wallet)
        {
            _analytics = analytics;
            _backendValidator = backendValidator;
            _inventory = inventory;
            _wallet = wallet;
        }

        public async Task<bool> PurchaseItemAsync(string iID, int price)
        {
            if(!_wallet.HasEnoughMoney(price))
            {
                _analytics.LogTransaction(iID, false);
                return false;
            }
            
            _wallet.Withdraw(price);

            try
            {
                bool isValid = await _backendValidator.ValidateReceiptAsync(iID);
                if (!isValid)
                {
                    Rollback(iID, price);
                    return false;
                }

                if (!_inventory.TryAddItem(iID))
                {
                    Rollback(iID, price);
                    return false;
                }
                
                _analytics.LogTransaction(iID, true);
                return true;
            }
            catch (Exception)
            {
                Rollback(iID, price);
                return false;
            }
        }

        private void Rollback(string iID, int price)
        {
            _wallet.Deposit(price);
            _analytics.LogTransaction(iID, false);
        }
    }
}