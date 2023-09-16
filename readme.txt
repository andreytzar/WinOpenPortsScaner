#WinOpenPortsScaner

This is a simple scanner of open TCP/UDP ports, with the ability to obtain basic information about processes and remote endpoints.

## PortScaner class

Emulates the 'netstat -ano' command. And it converts the resulting list of strings into a list of PortInfo objects.

## PortInfo

This class provides basic information about the open port, local and remote point. The class has static methods for obtaining basic information about the remote host and the process associated with the port.