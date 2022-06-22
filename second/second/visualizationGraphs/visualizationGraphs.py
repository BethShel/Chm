import json
import matplotlib.pyplot as plt

def lineplot(x_data, y_data, z_data, x_data3, y_data3, z_data3):
    _, ax = plt.subplots()
    
    plt.plot(x_data, y_data, 'y')
    plt.plot(x_data, z_data, 'g')
    plt.plot(x_data3, y_data3, 'r')
    plt.plot(x_data3, z_data3, 'b')
    plt.show()


with open("D:\\Лиза\\ВУЗ\\ЧМ\\second\\second\\Save\\temp.json", "r") as read_file:
    data = json.load(read_file)
lineplot(data['X1'], data['Y1'], data['Z1'], data['X3'], data['Y3'], data['Z3'])