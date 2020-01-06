# CNAG: simplistic port knocker

## Purpose

This tool allows to issue "port-knocking" sequence for communicating with programs such as [knockd](https://zeroflux.org/projects/knock). The tool is very simple and can perform only TCP/SYN knocks.

**While the tool is written in .NET Core, I have tested it only on Windows. It may or may not work under Linux and/or macOs.**

## Installation

[.NET Core Runtime](https://docs.microsoft.com/en-us/dotnet/core/install/runtime) is required.

To install `cnag` run

`dotnet tool install --global cnag`

You can also compile it from the source which is available at https://github.com/avysk/cnag.

## Usage

`cnag <my.host.name> <port1> <port2> ...`

You can specify optional delay (in ms) between connection attempts using `-d <delay>` option; the default value is 100 ms.

The tool is silent by default; add `-v` option for some verbosity.

## License: BSD 2-clause

Copyright 2020 Alexey Vyskubov <alexey@hotmail.fi>

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
