using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using McMaster.Extensions.CommandLineUtils;

namespace cnag
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        protected Program() {}
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Verbose messages; if option is not given, the tool is silent.")]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        private static bool Verbose { get; }

        [Option(Description = "Delay between connection attempts, in ms. Default: 100 ms.")]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        private static int? Delay { get; }

        [Argument(0, Description = "Host name to initiate connection attempts with")]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        private static string HostName { get; }

        [Argument(1, Description = "port numbers to initiate connections to")]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        private static int[] Ports { get; }

        // ReSharper disable once UnusedMember.Local
#pragma warning disable S1144
        private static int OnExecute()
#pragma warning enable S1144
        {
            int delay = Delay ?? 100;

            if (HostName == null)
            {
                Console.Error.WriteLine("No hostname is given.");
                return 1;
            }

            if (Ports == null)
            {
                Console.Error.WriteLine("At least one port must be given.");
                return 1;
            }

            IPHostEntry hosts;

            try
            {
                hosts = Dns.GetHostEntry(HostName);
            }
            catch (SocketException)
            {
                Console.Error.WriteLine($"Cannot resolve '{HostName}'.");
                return 1;
            }

            if (hosts.AddressList.Length == 0)
            {
                Console.Error.WriteLine($"Cannot resolve '{HostName}'.");
                return 1;
            }

            var address = hosts.AddressList[0];
            if (Verbose)
            {
                Console.WriteLine($"Port-knocking {address}...");
            }

            foreach (var port in Ports)
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(hosts.AddressList[0], port);
                if (Verbose)
                {
                    Console.WriteLine($"Knock-knock, port {port}!");
                }
                sock.BeginConnect(endPoint, res => { }, sock);
                sock.Close();
                Thread.CurrentThread.Join(delay);
            }

            return 0;
        }
    }
}
