#! /usr/bin/env python

import os
import signal
import socket
#https://qiita.com/methane/items/a467a28c8359b045a498

signal.signal(signal.SIGPIPE, signal.SIG_IGN)

sock = socket.socket(socket.AF_UNIX)
os.unlink('test.uds')
sock.bind('test.uds')
sock.listen(1)
print("socket listen")

while True:
    s, addr = sock.accept()
    
    while True:
        s.sendall("a"*60 + '\n')
