string = "0,0,0,0,"
str_list = string.split(',')
str_list.pop(len(str_list) -1 )

for i in str_list:
    print(i)
print("end")