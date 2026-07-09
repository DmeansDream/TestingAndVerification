namespace ScriptsToTest.Mock
{
    public interface IAnalytics
    {
        public void LogTransaction(string iID, bool isSuccess);
    }
}