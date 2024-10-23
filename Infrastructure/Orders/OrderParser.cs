using System.Globalization;
using Core.Orders;
using Serilog;

namespace Infrastructure.Orders;

public class OrderParser : IOrderParser
{
    public bool TryParseOrder(string line, out Order? order)
    {
        order = null;
        var fields = line.Split(';');
        
        if (fields.Length != 4)
        {
            Log.Warning("Некорректные данные в строке: " + line);
            return false;
        }

        if (!int.TryParse(fields[0], out var orderId))
        {
            Log.Warning($"Некорректный идентификатор заказа: {fields[0]}");
            return false;
        }

        if (!double.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var weight))
        {
            Log.Warning($"Некорректный вес: {fields[1]}");
            return false;
        }

        if (!DateTime.TryParse(fields[3], out var deliveryTime))
        {
            Log.Warning($"Некорректное время доставки: {fields[3]}");
            return false;
        }

        order = new Order
        {
            Id = orderId,
            WeightKg = weight,
            District = fields[2],
            DeliveryTime = deliveryTime
        };

        return true;
    }
}