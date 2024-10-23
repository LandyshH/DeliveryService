using Core.Orders;

namespace Infrastructure.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly IOrderParser _orderParser;

    public OrderRepository(IOrderParser orderParser)
    {
        _orderParser = orderParser;
    }

    public List<Order> GetOrders(string[] lines)
    {
        var orders = new List<Order>();

        foreach (var line in lines)
        {
            if (!_orderParser.TryParseOrder(line, out var order))
            {
                continue;
            }

            if (order != null)
            {
                orders.Add(order);
            }
        }

        return orders;
    }

    public List<Order> FilterOrdersByDistrictAndTime(List<Order> orders, string district,
                                                        DateTime firstDeliveryTime, TimeSpan interval)
    {
        return orders
            .Where(order => order.District == district && 
                            order.DeliveryTime >= firstDeliveryTime &&
                            order.DeliveryTime <= firstDeliveryTime.Add(interval))
            .ToList();
    }
}