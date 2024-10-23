namespace Infrastructure.Configurations;

public class OrderOptions
{
    public required string CityDistrict { get; set; }
    public required string DeliveryLog { get; set; }
    public required string DeliveryOrder { get; set; }
    public required string FirstDeliveryDateTime { get; set; }
}