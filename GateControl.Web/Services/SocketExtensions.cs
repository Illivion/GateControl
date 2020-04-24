using System;
using System.Net.Sockets;
using System.Text;

namespace GateControl.Web.Services
{
    public static class SocketExtensions
    {
        public static void SendString(this Socket socket, String value)
        {
            socket.Send(Encoding.ASCII.GetBytes(value + "\n"));
        }
    }
}