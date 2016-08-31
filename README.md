NAT Hole Punching example written in C# using the UDT library - https://github.com/dump247/udt-net

## What it does
Very simple: shows how to punch a hole in the NAT using the standard RENDEZVOUZ method in UDT.

The example connects to peers over the internet even when they are behind firewalls.

Components:

* Server code: runs on an internet machine accessible to both peers. Its only mission is to exchange the public ips and ports between the two peers.
* Client code: the one that will run in the two clients, each behind a different firewall. It contains code to get the peer public address (getting it from the server), and then the code to open a connection in UDT RENDEZVOUS mode. Then it includes same sample code to send/receive data.

##How to run it

###Server
Start it on a machine accessible by the two peers. Typically an Amazon/Azure host will do (remember to open the correct ports).

udtholepunch server PORT_NUMBER

where PORT_NUMBER is the port you want to use to listen and the one the clients will use to connect.

###Client

udtholepunch client local_port server server_port role

where

local_port: is the local port that you will use to bind. Remember that this is going to be used when you connect to the server, and again when you try to connect to the peer, so that the same NAT mapping is preserved.

server: the server IP (IP, not name, name resolution not implemented)

server_port

role: client|server, whether you want your client to send or receive data... this is just a convention for the sake of simplicity in the example.


You will have to start up two clients on two different machines to test the whole thing.


Enjoy!
