using System;
using System.IO;

using System.Net;
using System.Net.Sockets;

using Udt;

namespace udtholepunch
{
    class Server
    {
        public static void Run(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: {0} server port",
                    System.AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            Udt.Socket socket = new Udt.Socket(AddressFamily.InterNetwork, SocketType.Stream);

            socket.Bind(IPAddress.Any, int.Parse(args[0]));

            socket.Listen(1);

            while (true)
            {
                Udt.Socket client0 = socket.Accept();

                Console.WriteLine("First connection accepted from {0}",
                    client0.RemoteEndPoint.ToString());

                Udt.Socket client1 = socket.Accept();

                Console.WriteLine("Second connection accepted from {0}",
                    client1.RemoteEndPoint.ToString());

                IPEndPoint client1EndPoint = client1.RemoteEndPoint;

                SendAddressTo(client0.RemoteEndPoint, client1);
                SendAddressTo(client1EndPoint, client0);
            }
        }

        static void SendAddressTo(IPEndPoint endPoint, Udt.Socket socket)
        {
            using (Udt.NetworkStream st = new Udt.NetworkStream(socket))
            using (BinaryWriter writer = new BinaryWriter(st))
            {
                byte[] addrBytes = endPoint.Address.GetAddressBytes();

                writer.Write((int)addrBytes.Length);
                writer.Write(addrBytes);
                writer.Write((int)endPoint.Port);
            }
        }
    }
}
