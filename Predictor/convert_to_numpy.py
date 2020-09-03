import numpy as np 

tensor = np.zeros((28, 28)).reshape(1,28,28,1)

print(tensor)
print(tensor.shape)

tensor = np.zeros(28*28)
tensor = tensor.reshape(1,28,28,1)
print(tensor)
print(tensor.shape)

