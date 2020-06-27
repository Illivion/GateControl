using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace GateControl.Web.GRPC
{
    public class GateControlGrpc : GateMessages.GateMessagesBase
    {
        private readonly OpenCloseCommandService _openCloseCommandService;

        public GateControlGrpc(OpenCloseCommandService openCloseCommandService)
        {
            _openCloseCommandService = openCloseCommandService;
        }

        public override async Task OpenCloseCommandStream(
            OpenCloseCommandStreamRequest request,
            IServerStreamWriter<OpenCloseCommand> responseStream,
            ServerCallContext context)
        {
            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await _openCloseCommandService.Wait(context.CancellationToken);

                    await responseStream.WriteAsync(new OpenCloseCommand());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERR] {ex.Message}");
            }
        }
    }
}
