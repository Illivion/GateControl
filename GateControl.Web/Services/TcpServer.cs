using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GateControl.Web.Services
{
    public class TcpServer
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly Socket _acceptingSocket;

        private Socket _currentClient = null;

        private Timer _pingTimer;

        private Object _sync = new Object();

        private Task _acceptingTask;

        public TcpServer(String ip, Int32 port)
        {
            _ip = ip;
            _port = port;
            _acceptingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _pingTimer = new Timer(DoPing);
        }

        private void DoPing(object? state)
        {
            _currentClient?.SendString("ping");
        }

        public Boolean SendPushCommand()
        {
            var client = _currentClient;

            try
            {
                client?.SendString("push");
            }
            catch
            {
                client?.Close();
            }

            return client != null;
        }

        public void Start()
        {
            _acceptingSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));

            _acceptingSocket.Listen(10);

            _acceptingTask = Task.Run(async () =>
            {
                while (true)
                {
                    var clientSocket = await _acceptingSocket.AcceptAsync();

                    _currentClient?.Close(1);

                    _currentClient = clientSocket;
                }
            });
        }

        public void Stop()
        {
            try
            {
                _acceptingSocket.Close();
                _acceptingSocket.Dispose();

                _currentClient?.Close();
                _currentClient?.Dispose();
            }
            catch { }
        }

        public Boolean IsDeviceConnected()
        {
            return _currentClient != null;
        }
    }
}