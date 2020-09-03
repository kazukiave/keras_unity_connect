import socket 


ip = '127.0.0.1'
port = 50007

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((ip, port))
    s.listen(1)

    while True:
        connection, address = s.accept()
        with connection:
            while True:
                data = connection.recv(1024)
                if not data:
                    break
                print('data : {}, addr: {}'.format(data, address))
                connection.sendall(b'Received: ' + data)