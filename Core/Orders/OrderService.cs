using Core.Files;
using Serilog;

namespace Core.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IFileReader _fileReader;
        private readonly IFileWriter _fileWriter;

        public OrderService (IOrderRepository orderRepository, IFileReader fileReader, IFileWriter fileWriter)
        {
            _orderRepository = orderRepository;
            _fileReader = fileReader;
            _fileWriter = fileWriter;
        }

        public void ProcessOrders(string baseDirectory, string cityDistrict, DateTime firstDeliveryDateTime,
            string deliveryOrderPath)
        {
            var orders = ReadOrdersFromFile(baseDirectory);

            if (orders.Count == 0)
            {
                Log.Warning("Нет заказов для обработки.");
                return;
            }

            FilterAndWriteOrders(orders, cityDistrict, firstDeliveryDateTime, deliveryOrderPath);
        }

        private List<Order> ReadOrdersFromFile(string baseDirectory)
        {
            var ordersFilePath = Path.Combine(baseDirectory, "Data", "orders_data.txt");

            if (!File.Exists(ordersFilePath))
            {
                Log.Error($"Файл заказов не найден: {ordersFilePath}");
                return [];
            }

            string[] ordersLines;
            try
            {
                ordersLines = _fileReader.ReadFileText(ordersFilePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при чтении файла заказов.");
                return [];
            }

            Log.Information("Получение данных о заказах.");
            var orders = _orderRepository.GetOrders(ordersLines);
            Log.Information($"Получено: {orders.Count} заказов.");
            return orders;
        }

        private void FilterAndWriteOrders(List<Order> orders,
            string cityDistrict, DateTime firstDeliveryDateTime, string deliveryOrderPath)
        {
            Log.Information("Фильтрация заказов.");
            var filteredOrders = _orderRepository.FilterOrdersByDistrictAndTime(orders, cityDistrict,
                firstDeliveryDateTime, TimeSpan.FromMinutes(30));

            if (filteredOrders.Count == 0)
            {
                Log.Warning("Не найдено заказов по указанным критериям.");
            }
            else
            {
                Log.Information($"Запись {filteredOrders.Count} отфильтрованных заказов в {deliveryOrderPath}");
                _fileWriter.WriteItemsListToFile(deliveryOrderPath, filteredOrders);
            }
        }
    }
}