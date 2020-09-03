"""
reference
https://pymotw.com/2/socket/uds.html
"""

import os 
import sys
import socket


server_address = 'test.uds'

#make sure socket does not already exist
try:
    os.unlink(server_address)
except:
    if os.path.exists(server_address):
        raise #call exception

#create unix domain socket
sock = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)

#bind the scoket to the port
print("starting up on" + server_address)
sock.bind(server_address)

#Listen for incoming connections
sock.listen(1)

while True:
    print("wait for a connection")
    connection, client_address = sock.accept()
    try:
        print("connection come from " + client_address)
        while True:
            data = connection.recv(1024)
            if data:
                print("server receive a data")
                message = str("received data is ").encode()
                connection.sendall(message + data)
            else:
                break
    finally:
        connection.close()
