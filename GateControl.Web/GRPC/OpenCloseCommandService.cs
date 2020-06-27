using System.Threading;
using System.Threading.Tasks;

namespace GateControl.Web.GRPC
{
    public class OpenCloseCommandService
    {
        private readonly SemaphoreSlim _sync;

        public OpenCloseCommandService()
        {
            _sync = new SemaphoreSlim(0,1);
        }

        public void SendCommand()
        {
            _sync.Release();
        }

        public async Task Wait(CancellationToken cancellationToken)
        {
            await _sync.WaitAsync(cancellationToken);
        }
    }
}