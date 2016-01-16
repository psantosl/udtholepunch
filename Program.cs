using System;
using System.Linq;

namespace udtholepunch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: {0} [client|server]",
                    System.AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            if (args[0] == "server")
            {
                Server.Run(args.Skip(1).ToArray());
            }

            Client.Run(args.Skip(1).ToArray());
        }
    }
}
