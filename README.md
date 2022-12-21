# CNAG: portable port knocker

![License](https://badgen.net/github/license/avysk/cnag?color=green&scale=1.2)

![GitHub Workflow Status](https://badgen.net/github/checks/avysk/cnag/%D0%BC%D0%B0%D1%81%D1%82%D0%B5%D1%80/build?label=build&scale=1.2)

![Latest tag](https://badgen.net/github/tag/avysk/cnag?scale=1.2)
![Nuget](https://badgen.net/nuget/v/cnag?scale=1.2)
![Nuget downloads](https://badgen.net/nuget/dt/cnag?scale=1.2)

If badges do not work, badgen is crashing. See how it looks in `badgen.png`
screenshot.

## Purpose

This tool allows to issue "port-knocking" sequence for communicating with
programs such as [knockd](https://github.com/jvinet/knock/). The tool is
very simple and can perform only TCP/SYN knocks over IPv4 or IPv6 -- and maybe UDP knocks as well.

**While the tool is written in .NET 6, I have tested it only on Windows. It may
or may not work under Linux and/or macOs.** It seems to work for me on FreeBSD
13.1.

## Installation

[.NET 6 Runtime is required.](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

To install: `dotnet tool install --global cnag`

You can also compile it from the source which is available
[in the repository.](https://github.com/avysk/cnag)

## Usage

See [another document.](Cnag/docs.md)
