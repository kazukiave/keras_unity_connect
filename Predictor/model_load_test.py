#!/Users/kazuki/opt/anaconda3/envs/keras_py36/bin/ python3

print("Start")
from keras.models import load_model
print(0)
from keras.preprocessing import image
print(1)
import numpy as np 
print(2)
import matplotlib as plt
print(3)

img_path = '/Users/kazuki/Documents/_DeepLearning/test1.png'
target_size = (28,28)


img = image.load_img(img_path, target_size=target_size, grayscale=True)

img_tensor = image.img_to_array(img)
print(img_tensor.shape)
img_tensor = np.expand_dims(img_tensor, axis=0)
img_tensor /= 255.

#print(img_tensor.shape)
#print(img_tensor)


model = load_model('/Users/kazuki/Documents/_DeepLearning/Mnist_Classification10.h5')
#print(model.summary())

predict = model.predict_classes(img_tensor)
text = str(predict[0])
print(text)

text_text = open("/Users/kazuki/Documents/_DeepLearning/test.txt", 'w')
text_text.write(text)