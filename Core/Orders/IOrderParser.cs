namespace Core.Orders;

public interface IOrderParser
{
    bool TryParseOrder(string line, out Order? order);
}