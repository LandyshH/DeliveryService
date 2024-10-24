using Core.Orders;
using Infrastructure.Orders;

namespace Tests.Infrastructure.Orders;

public class OrderParserTests
{
    private readonly OrderParser _orderParser;

    public OrderParserTests ()
    {
        _orderParser = new OrderParser();
    }

    [Fact]
    public void TryParseOrder_WhenLineIsValid_ShouldReturnTrueAndOrderObject()
    {
        // Arrange
        const string line = "1;2.5;DistrictA;2024-10-01 10:00:00";
        var expectedOrder = new Order
        {
            Id = 1,
            WeightKg = 2.5,
            District = "DistrictA",
            DeliveryTime = DateTime.Parse("2024-10-01 10:00:00")
        };
        
        // Act
        var result = _orderParser.TryParseOrder(line, out var actualOrder);

        // Assert
        Assert.True(result);
        Assert.NotNull(actualOrder);
        Assert.Equal(expectedOrder, actualOrder);
    }
    
    [Fact]
    public void TryParseOrder_WhenLineNotValid_ShouldReturnFalse()
    {
        // Arrange
        const string line = "line";

        // Act
        var result = _orderParser.TryParseOrder(line, out var actualOrder);

        // Assert
        Assert.False(result);
        Assert.Null(actualOrder);
    }
    
    [Fact]
    public void TryParseOrder_WhenOrderIdNotValid_ShouldReturnFalse()
    {
        // Arrange
        const string line = "Id;2.5;DistrictA;2024-10-01 10:00:00";

        // Act
        var result = _orderParser.TryParseOrder(line, out var actualOrder);

        // Assert
        Assert.False(result);
        Assert.Null(actualOrder);
    }

    [Fact]
    public void TryParseOrder_WhenWeightNotValid_ShouldReturnFalse()
    {
        // Arrange
        const string line = "1;Weight;DistrictA;2024-10-01 10:00:00";

        // Act
        var result = _orderParser.TryParseOrder(line, out var actualOrder);

        // Assert
        Assert.False(result);
        Assert.Null(actualOrder);
    }

    [Fact]
    public void TryParseOrder_WhenDeliveryTimNotValid_ShouldReturnFalse()
    {
        // Arrange
        const string line = "1;2.5;DistrictA;Time";

        // Act
        var result = _orderParser.TryParseOrder(line, out var actualOrder);

        // Assert
        Assert.False(result);
        Assert.Null(actualOrder);
    }
}