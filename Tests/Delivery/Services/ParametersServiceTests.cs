using Delivery.Services;
using Infrastructure.Configurations;

namespace Tests.Delivery.Services;

public class ParametersServiceTests
{
    private readonly ParametersService _parametersService;

    public ParametersServiceTests()
    {
        _parametersService = new ParametersService();
    }

    private const string BaseDirectory = "";
    private const string District = "DistrictA";
    private const string DeliveryLog = "delivery.txt";
    private const string DeliveryOrder = "deliveryOrder.txt";
    private const string FirstDeliveryDateTime = "2024-10-24 10:00:00";
    
    [Fact]
    public void LoadAndValidateParameters_WhenValidParametersProvidedFromConfiguration_ShouldReturnParameters()
    {
        // Arrange
        var orderConfig = new OrderOptions
        {
            CityDistrict = District,
            DeliveryLog = DeliveryLog,
            DeliveryOrder = DeliveryOrder,
            FirstDeliveryDateTime = FirstDeliveryDateTime
        };

        var args = Array.Empty<string>();

        // Act
        var result = _parametersService.LoadAndValidateParameters(orderConfig, args, BaseDirectory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(District, result.Value.cityDistrict);
        Assert.Equal(DeliveryLog, result.Value.deliveryLogPath);
        Assert.Equal(DeliveryOrder, result.Value.deliveryOrderPath);
        Assert.Equal(DateTime.Parse(FirstDeliveryDateTime), result.Value.firstDeliveryDateTime);
    }
    
    [Fact]
    public void LoadAndValidateParameters_WhenValidParametersProvidedFromArgs_ShouldReturnParameters()
    {
        // Arrange
        string[] args =
        [
            "_cityDistrict",
            District,
            "_firstDeliveryDateTime",
            FirstDeliveryDateTime,
            "_deliveryLog",
            DeliveryLog,
            "_deliveryOrder",
            DeliveryOrder
        ];

        // Act
        var result = _parametersService.LoadAndValidateParameters(null, args, BaseDirectory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(District, result.Value.cityDistrict);
        Assert.Equal(DeliveryLog, result.Value.deliveryLogPath);
        Assert.Equal(DeliveryOrder, result.Value.deliveryOrderPath);
        Assert.Equal(DateTime.Parse(FirstDeliveryDateTime), result.Value.firstDeliveryDateTime);
    }
    
    [Fact]
    public void LoadAndValidateParameters_WhenInvalidParameters_ShouldReturnNull()
    {
        // Arrange
        var orderConfig = new OrderOptions
        {
            CityDistrict = "",
            DeliveryLog = DeliveryLog,
            DeliveryOrder = DeliveryOrder,
            FirstDeliveryDateTime = FirstDeliveryDateTime
        };

        var args = Array.Empty<string>();

        // Act
        var result = _parametersService.LoadAndValidateParameters(orderConfig, args, BaseDirectory);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void LoadAndValidateParameters_WhenInvalidDeliveryDate_ShouldReturnNull()
    {
        // Arrange
        var orderConfig = new OrderOptions
        {
            CityDistrict = District,
            DeliveryLog = DeliveryLog,
            DeliveryOrder = DeliveryOrder,
            FirstDeliveryDateTime = "FirstDeliveryDateTime"
        };

        var args = Array.Empty<string>();

        // Act
        var result = _parametersService.LoadAndValidateParameters(orderConfig, args, BaseDirectory);

        // Assert
        Assert.Null(result);
    }
}