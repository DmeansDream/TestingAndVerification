namespace ScriptsToTest.Mock
{
    public interface IInventory
    {
        public bool TryAddItem(string iID);
        public bool TryGetItemPrice(string itemId, out int price);
    }
}