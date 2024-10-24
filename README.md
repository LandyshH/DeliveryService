Запуск программы: 
1. Delivery.exe в папке publish 

2. IDE: запуск проекта Delivery,
     либо из папки проекта, сразу передавая параметры командой

   **dotnet run -- _cityDistrict [district] _firstDeliveryDateTime [date] _deliveryLog [path] _deliveryOrder [path])**

Пример ввода параметров: 

 _cityDistrict "DistrictA" _firstDeliveryDateTime "2024-10-01 13:00:00" _deliveryLog [path] _deliveryOrder [path]
 
Если введены не все значения, то недостающие подгрузятся из конфигурации. 

Данные о заказах лежат в файле Infrastructure/Data/orders_data.txt (либо publish/Data)
