using System;
using System.Net.Sockets;
using System.Text;

namespace GateControl.Web.Services
{
    public static class SocketExtensions
    {
        public static Int32 SendString(this Socket socket, String value)
        {
            var cmd = Encoding.ASCII.GetBytes(value + "\n");

            socket.Send(cmd);

            return cmd.Length;
        }
    }
}