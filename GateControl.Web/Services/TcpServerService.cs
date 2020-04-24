using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GateControl.Web.Services
{
    public class TcpServerService : IHostedService
    {
        private readonly TcpServer _server;

        public TcpServerService()
        {
            _server = new TcpServer(Settings.IP, Settings.TcpPort);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _server.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _server.Stop();
        }

        public Boolean Push()
        {
            return _server.SendPushCommand();
        }
    }
}
