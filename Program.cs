using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using McMaster.Extensions.CommandLineUtils;

namespace cnag
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Verbose messages; if option is not given, the tool is silent.")]
        private bool Verbose { get; set; }

        [Option(Description = "Delay between connection attempts, in ms. Default: 100 ms.")]
        private Nullable<int> Delay { get; }

        [Argument(0, Description = "Host name to initiate connection attempts with")]
        private string HostName { get; }

        [Argument(1, Description = "port numbers to initiate connections to")]
        private int[] Ports { get; }

        private int OnExecute()
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
                Console.Error.WriteLine(String.Format("Cannot resolve '{0}'.", HostName));
                return 1;
            }

            if (hosts.AddressList.Length == 0)
            {
                Console.Error.WriteLine(String.Format("Cannot resolve '{0}'.", HostName));
                return 1;
            }

            var address = hosts.AddressList[0];
            if (Verbose)
            {
                Console.WriteLine(String.Format("Port-knocking {0}...", address));
            }

            foreach (var port in Ports)
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(hosts.AddressList[0], port);
                if (Verbose)
                {
                    Console.WriteLine(String.Format("Knock-knock, port {0}!", port));
                }
                sock.BeginConnect(endPoint, new AsyncCallback(res => { }), sock);
                sock.Close();
                Thread.CurrentThread.Join(delay);
            }

            return 0;
        }
    }
}
