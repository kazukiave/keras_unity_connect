#!/Users/kazuki/opt/anaconda3/envs/keras_py36/bin/ python3


from keras.models import load_model
from keras.preprocessing import image
import numpy as np
import matplotlib as plt
import socket
import sys

ip = '127.0.0.1'
port = 50006
desired = 0
step = 0


def classify_tensor(tensor):
    tensor_list = tensor.split(',')
    tensor_list.pop(len(tensor_list)-1)#split するとき最後が',’で終わってると勝手に空白のstr入るからそれpopで消す。
    tensor_arr = np.zeros(28 * 28, dtype=np.float)
   
    for i in range(len(tensor_list)):
        tensor_arr[i] = float(tensor_list[i])
    tensor_arr = tensor_arr.reshape(1,28,28,1)
  
    predict = model.predict((tensor_arr))[0]
   # predict_number = model.predict_classes(tensor_arr)[0]
    #print(predict)
   # print("Desired is " + str(desired) + "predict is " + str(predict_number))
   # print("step count" + str(step))
    return_value = round(predict[desired], 5)
    print(str(return_value) + " probability")
    print()
    return return_value
   

def classify_images():
    img = image.load_img(img_path, target_size=target_size, grayscale=True)
    img_tensor = image.img_to_array(img)
    img_tensor = np.expand_dims(img_tensor, axis=0)
    img_tensor /= 255.

    '''
    print(img_tensor.shape)
    print(img_tensor)
    '''

    predict = model.predict((img_tensor))[0]
    predict_number = model.predict_classes(img_tensor)[0]
    print(predict)
    print("predict is " + str(predict_number))
    
    print("step count" + str(step))
    #predict = model.predic_classes(img_tensor)
    return str(predict[1])


img_path = './agentView.png'
model = load_model('/Users/kazuki/Documents/_DeepLearning/Mnist_Classification10.h5')
target_size = (28,28)

if len(sys.argv) == 1:
    print("No argments have been enterd,If you wnat to select a port, please put it in")

port = int(sys.argv[1])
print("port is" + str(port))

#create tcp socket
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((ip, port))
    s.listen(1)
    print("desired number is " + str(desired))
    print("wait for connection")
    
    #wait for connection
    while True:
        connection, address = s.accept()
        with connection:
            while True:
                data = connection.recv(1570)
                if not data:
                    break
                #if recive data then return predict
                
                predict = float(classify_tensor(data.decode('utf-8')))
                
                step = step + 1
                connection.sendall(str(predict).encode())