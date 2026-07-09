using System;

namespace ScriptsToTest.Mock
{
    public interface IWallet
    {
        public bool HasEnoughMoney(int amount);
        public void Withdraw(int amount);
        public void Deposit(int amount);
        public event Action<int> OnBalanceChanged;
    }
}