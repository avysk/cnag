using Serilog;
using Serilog.Core;

using System;
using System.CommandLine;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cnag;

/// <summary>
/// The program itself.
/// </summary>
public static class Program
{
    /// <summary>
    /// The entry point.
    /// </summary>
    /// <param name="args">Command-line options and arguments.</param>
    public static void Main(string[] args)
    {
        Option<bool> verboseOption =
            new("--verbose", "Produce verbose messages.");

        Option<bool> ipv4Option = new("--ipv4", "Use only IPv4 address.");
        ipv4Option.AddAlias("-4");

        Option<bool> udpOption = new("--udp", "Use UDP for knocking.");
        udpOption.AddAlias("-u");

        Option<int> delayOption =
            new(
                "--delay",
                () => 100,
                "Delay between issuing port-knocking sequences, in ms."
            );
        delayOption.AddAlias("-d");
        delayOption.AddValidator(result =>
        {
            if (result.GetValueForOption(delayOption) == 0)
            {
                result.ErrorMessage = "The delay must be greater than 0.";
            }
        });
        Argument<string> hostArgument =
            new("hostname", "The name of the host to port-knock to.");
        hostArgument.AddValidator(result =>
        {
            if (result.GetValueForArgument(hostArgument)?.Length == 0)
            {
                result.ErrorMessage = "The host name cannot be empty";
            }
        });
        Argument<ushort[]> portsArgument =
            new("ports", "The ports to port-knock.")
            {
                Arity = ArgumentArity.OneOrMore
            };
        portsArgument.AddValidator(result =>
        {
            var value = result.GetValueForArgument(portsArgument);
            if (value.Any(p => p == 0))
            {
                result.ErrorMessage = "A port number cannot be 0.";
            }
        });

        RootCommand rootCommand =
            new()
            {
                verboseOption,
                ipv4Option,
                udpOption,
                delayOption,
                hostArgument,
                portsArgument
            };
        rootCommand.SetHandler(
            (verbose, ipv4, udp, delay, hostName, ports) =>
            {
                var lc = new LoggerConfiguration();
                _ = verbose
                    ? lc.MinimumLevel.Verbose()
                    : lc.MinimumLevel.Warning();

#pragma warning disable CA1305
                Logger log = lc.WriteTo.Console().CreateLogger();
#pragma warning restore CA1305
                log.Verbose("The port-knocker is started.");
                log.Verbose(
                    $"Options and arguments: verbose {verbose}, "
                        + $"ipv4 {ipv4}, "
                        + $"udp {udp}, "
                        + $"delay {delay}ms, "
                        + $"hostname {hostName}, "
                        + $"ports {string.Join(',', ports)}."
                );

                log.Debug($"Resolving {hostName}...");
                IPAddress address = GetAddress(hostName, ipv4, log);

                SocketType socketType = udp
                    ? SocketType.Dgram
                    : SocketType.Stream;
                ProtocolType protocolType = udp
                    ? ProtocolType.Udp
                    : ProtocolType.Tcp;

                log.Debug($"Got {address} for {hostName}.");
                foreach (var port in ports)
                {
                    var sock = new Socket(
                        address.AddressFamily,
                        socketType,
                        protocolType
                    );
                    IPEndPoint endPoint = new(address, port);

                    log.Verbose($"Knock-knock, port {port}!");

                    _ = sock.BeginConnect(endPoint, _ => { }, sock);
                    sock.Close();
                    _ = Thread.CurrentThread.Join(delay);
                }
            },
            verboseOption,
            ipv4Option,
            udpOption,
            delayOption,
            hostArgument,
            portsArgument
        );

        _ = rootCommand.Invoke(args);
        Environment.Exit(0);
    }

    private static IPAddress GetAddress(string host, bool ipv4Only, Logger log)
    {
        IPHostEntry hosts = new();
        try
        {
            hosts = Dns.GetHostEntry(host);
        }
        catch (SocketException)
        {
            log.Error($"Cannot resolve '{host}'.");
            Environment.Exit(-1);
        }

        if (hosts.AddressList.Length == 0)
        {
            log.Error($"Address list for '{host}' is empty.");
            Environment.Exit(-1);
        }

        foreach (IPAddress ipaddr in hosts.AddressList)
        {
            if (ipv4Only && ipaddr.AddressFamily != AddressFamily.InterNetwork)
            {
                log.Debug($"Skipping non-IPv4 address {ipaddr}");
                continue;
            }

            log.Debug($"Found address {ipaddr}");
            return ipaddr;
        }

        log.Error($"Cannot find IPv4 address for {host}.");
        Environment.Exit(-1);
        return null; // unreachable
    }
}
