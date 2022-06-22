import json
import matplotlib.pyplot as plt

def lineplot(x_data, y_data, x_data3, y_data3):
    _, ax = plt.subplots()
    
    plt.plot(x_data, y_data, 'b')
    plt.plot(x_data3, y_data3, 'm')
    plt.show()


with open("D:\\Лиза\\ВУЗ\\ЧМ\\second1\\second1\\Save\\temp.json", "r") as read_file:
    data = json.load(read_file)
lineplot(data['X1'], data['Y1'], data['X3'], data['Y3'])