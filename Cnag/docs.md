# Usage

`cnag <my.host.name> <port1> <port2> ...`

You can specify optional delay (in ms) between connection attempts using `-d <delay>` option; the default value is 100 ms.

The tool is silent by default; add `--verbose` option if you wish to see a lot
of messages.

The parameter `-u` (stands for _untested_) switches to UDP mode.

The parameter `-4` allows to force IPv4 usage (by ignoring IPv6 addresses).
