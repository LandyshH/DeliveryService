namespace Core.Orders;

public interface IOrderService
{
    void ProcessOrders(string baseDirectory, string cityDistrict, DateTime firstDeliveryDateTime,
        string deliveryOrderPath);
}