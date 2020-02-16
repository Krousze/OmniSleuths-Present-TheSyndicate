using System.Threading.Tasks;

namespace TheSyndicate.Actions
{
    public interface IAction
    {
        public Task ExecuteActionAsync();
        public int GetIndexOfDestinationBasedOnUserSuccessOrFail();
        public bool DidPlayerSucceed();
    }
}
