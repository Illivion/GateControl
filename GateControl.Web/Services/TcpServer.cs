using System;
using System.Diagnostics;
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
        
        private Task _acceptingTask;

        private Object _obj = new Object();
        
        public TcpServer(String ip, Int32 port)
        {
            _ip = ip;
            _port = port;
            _acceptingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        private static Int32 _commandSequence = 1;
        
        public Boolean SendPushCommand()
        {
            var sw = Stopwatch.StartNew();

            lock (_obj)
            {
                var client = _currentClient;

                if (client == null)
                {
                    sw.Stop();

                    Debug.WriteLine(sw.ElapsedMilliseconds);

                    return false;
                }

                var cmdID = Interlocked.Increment(ref _commandSequence).ToString();

                try
                {
                    var command = $"push-{cmdID}";

                    var cmdSize = client.SendString(command);

                    var receiveBuffer = new Byte[cmdSize];

                    var received = 0;

                    var task = Task.Run(() =>
                    {
                        while (true)
                        {
                            try
                            {
                                var receivedBytes =
                                    client.Receive(receiveBuffer, received, 1, SocketFlags.None);

                                received += receivedBytes;

                                if (received == cmdSize-1)
                                {
                                    Debug.WriteLine($"{DateTime.Now}: ACK");
                                    break;
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                    });
                    
                    var ack = task.Wait(TimeSpan.FromSeconds(3));

                    sw.Stop();

                    Debug.WriteLine(sw.ElapsedMilliseconds);

                    return ack;
                }
                catch
                {
                    Close();

                    sw.Stop();

                    Debug.WriteLine(sw.ElapsedMilliseconds);

                    return true;
                }
            }
        }

        private void Close()
        {
            try
            {
                _currentClient?.Close(10);
                _currentClient?.Dispose();
            }
            catch { }
            finally
            {
                _currentClient = null;
            }
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

                    Close();

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

                Close();
            }
            catch { }
        }

        public Boolean IsDeviceConnected()
        {
            return _currentClient != null;
        }
    }
}