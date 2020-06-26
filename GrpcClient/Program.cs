using System;
using System.Threading.Tasks;
using GateControl.Web.GRPC;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    var channel = GrpcChannel.ForAddress("http://localhost:20080", new GrpcChannelOptions()
                    {
                        //Credentials = ChannelCredentials.Insecure
                    });

                    

                    var client = new GateMessages.GateMessagesClient(channel);

                    using var call = client.OpenCloseCommandStream(new OpenCloseCommandStreamRequest());

                    Console.WriteLine("Call accepted");

                    var stream = call.ResponseStream;

                    await foreach (var item in stream.ReadAllAsync())
                    {
                        Console.WriteLine("PUSH");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Disconnected");
                }
            }
        }
    }
}
