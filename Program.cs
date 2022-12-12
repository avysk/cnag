namespace Cnag;

using System;
using System.CommandLine;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Serilog;
using Serilog.Core;

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
            if (result.GetValueForArgument(hostArgument) == string.Empty)
            {
                result.ErrorMessage = "The host name cannot be empty";
            }
        });
        Argument<ushort[]> portsArgument =
            new("ports", "The ports to port-knock.");
        portsArgument.Arity = ArgumentArity.OneOrMore;
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
                delayOption,
                hostArgument,
                portsArgument,
            };
        rootCommand.SetHandler(
            (verbose, ipv4, delay, hostName, ports) =>
            {
                var lc = new LoggerConfiguration();
                if (verbose)
                {
                    lc.MinimumLevel.Verbose();
                }
                else
                {
                    lc.MinimumLevel.Warning();
                }

                var log = lc.WriteTo.Console().CreateLogger();
                log.Verbose("The port-knocker is started.");
                log.Verbose(
                    $"Options and arguments: verbose {verbose} ipv4 {ipv4}, "
                        + $"delay {delay}ms, hostname {hostName}, "
                        + $"ports {string.Join(',', ports)}."
                );

                log.Debug($"Resolving {hostName}...");
                var address = GetAddress(hostName, ipv4, log);

                log.Debug($"Got {address} for {hostName}.");
                foreach (var port in ports)
                {
                    var sock = new Socket(
                        address.AddressFamily,
                        SocketType.Stream,
                        ProtocolType.Tcp
                    );
                    IPEndPoint endPoint = new(address, port);

                    log.Verbose($"Knock-knock, port {port}!");

                    sock.BeginConnect(endPoint, _ => { }, sock);
                    sock.Close();
                    Thread.CurrentThread.Join(delay);
                }
            },
            verboseOption,
            ipv4Option,
            delayOption,
            hostArgument,
            portsArgument
        );

        rootCommand.Invoke(args);
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

        foreach (var ipaddr in hosts.AddressList)
        {
            if (ipv4Only && ipaddr.AddressFamily != AddressFamily.InterNetwork)
            {
                log.Debug($"Skipping non-IPv4 address {ipaddr}");
                continue;
            }

            log.Debug($"Found address {ipaddr}");
            return ipaddr;
        }

        throw new System.Diagnostics.UnreachableException();
    }
}
