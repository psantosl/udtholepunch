using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using Udt;

namespace udtholepunch
{
    class Client
    {
        public static void Run(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: {0} client local_port server server_port [client|server]",
                    System.AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            Udt.Socket client = new Udt.Socket(AddressFamily.InterNetwork, SocketType.Stream);
            client.ReuseAddress = true;

            client.Bind(IPAddress.Any, int.Parse(args[0]));

            IPAddress serverAddress;

            if (!IPAddress.TryParse(args[1], out serverAddress))
            {
                Console.WriteLine("Error trying to parse {0}", args[1]);
                return;
            }

            client.Connect(serverAddress, int.Parse(args[2]));

            int peerPort;
            IPAddress peerAddress;

            // recv the other peer info
            using (Udt.NetworkStream st = new Udt.NetworkStream(client, false))
            using (BinaryReader reader = new BinaryReader(st))
            {
                int len = reader.ReadInt32();

                byte[] addr = reader.ReadBytes(len);

                peerPort = reader.ReadInt32();

                peerAddress = new IPAddress(addr);

                Console.WriteLine("Received peer address = {0}:{1}",
                    peerAddress, peerPort);
            }

            bool bConnected = false;
            int retry = 0;

            while (!bConnected)
            try
            {
                client.Close();

                client = new Udt.Socket(AddressFamily.InterNetwork, SocketType.Stream);
                client.ReuseAddress = true;

                client.SetSocketOption(Udt.SocketOptionName.Rendezvous, true);

                client.Bind(IPAddress.Any, int.Parse(args[0]));

                Console.WriteLine("{0} - Trying to connect to {1}:{2}. ",
                    retry++, peerAddress, peerPort);

                client.Connect(peerAddress, peerPort);

                Console.WriteLine("Connected successfully to {0}:{1}",
                    peerAddress, peerPort);

                bConnected = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (args[3] == "client")
            {
                using (Udt.NetworkStream st = new Udt.NetworkStream(client))
                using (BinaryReader reader = new BinaryReader(st))
                {
                    while (true)
                    {
                        Console.WriteLine(reader.ReadString());
                    }
                }
            }
            else
            {
                using (Udt.NetworkStream st = new Udt.NetworkStream(client))
                using (BinaryWriter writer = new BinaryWriter(st))
                {
                    int last = Environment.TickCount;

                    while (!Console.KeyAvailable)
                    {
                        if (Environment.TickCount - last < 1000)
                            continue;

                        writer.Write(string.Format("[{0}] my local time is {1}",
                            Environment.MachineName, DateTime.Now.ToLongTimeString()));

                        last = Environment.TickCount;

                        TraceInfo traceInfo = client.GetPerformanceInfo();

                        Console.WriteLine("Bandwith Mbps {0}", traceInfo.Probe.BandwidthMbps);
                    }
                }
            }
        }
    }
}
