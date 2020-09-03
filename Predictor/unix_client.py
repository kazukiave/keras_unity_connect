#! /usr/bin/env python

import socket
import sys

sock =  socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)

server_address = 'test.uds'

print("connecting to a" + server_address)
try:
    sock.connect(server_address)
except:
    print("failed to connect server")

try:
    #send data
    message = "Hello"
    print("send a " + message)
    sock.sendall( message.encode())

    print("wait a receive mess")
    while True:
        data = sock.recv(1024)
        if not data:
            break
        print(data)

finally:
    sock.close()



    