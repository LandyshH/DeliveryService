using System.Globalization;

namespace Core.Orders;

public record Order
{
    public int Id { get; set; }
    public double WeightKg { get; set; }
    public string District { get; set; } = null!;
    public DateTime DeliveryTime { get; set; }
    
    public override string ToString()
    {
        return $"{Id};{WeightKg.ToString(CultureInfo.InvariantCulture)};{District};{DeliveryTime:yyyy-MM-dd HH:mm:ss}";
    }
}