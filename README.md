# CNAG: simplistic port knocker

## Purpose

This tool allows to issue "port-knocking" sequence for communicating with programs such as [knockd](https://zeroflux.org/projects/knock). The tool is very simple and can perform only TCP/SYN knocks.

**While the tool is written in .NET Core, I have tested it only on Windows. It may or may not work under Linux and/or macOs.**

## Installation

`dotnet tool install --global cnag`

You can also compile it from the source.

## Usage

`cnag <my.host.name> <port1> <port2> ...`

You can specify optional delay (in ms) between connection attempts using `-d <delay>` option; the default value is 100 ms.

The tool is silent by default; add `-v` option for some verbosity.
