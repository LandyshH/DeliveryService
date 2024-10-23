namespace Core.Orders;

public interface IOrderRepository
{
    List<Order> GetOrders(string[] lines);

    List<Order> FilterOrdersByDistrictAndTime(List<Order> orders, string district,
                                                DateTime firstDeliveryTime, TimeSpan interval);
}