using System.Threading.Tasks;

namespace ScriptsToTest.Mock
{
    public interface IBackendValidator
    {
        public Task<bool> ValidateReceiptAsync(string iId);
    }
}