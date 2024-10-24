using Core.Orders;
using Infrastructure.Orders;
using Moq;

namespace Tests.Infrastructure.Orders;

public class OrderRepositoryTests
{
    private readonly Mock<IOrderParser> _mockOrderParser;
    private readonly OrderRepository _orderRepository;

    public OrderRepositoryTests()
    {
        _mockOrderParser = new Mock<IOrderParser>();
        _orderRepository = new OrderRepository(_mockOrderParser.Object);
    }

    [Fact]
    public void GetOrders_WhenLinesAreValid_ShouldReturnListOfOrders()
    {
        // Arrange
        var lines = new[] { "1;2.5;DistrictA;2024-10-01 10:00:00", "2;1.0;DistrictB;2024-10-01 10:15:00" };
        var order1 = new Order { Id = 1, District = "DistrictA", DeliveryTime = DateTime.Parse("2024-10-01 10:00:00") };
        var order2 = new Order { Id = 2, District = "DistrictB", DeliveryTime = DateTime.Parse("2024-10-01 10:15:00") };

        _mockOrderParser.Setup(x => x.TryParseOrder(lines[0], out order1))
            .Returns(true);
        
        _mockOrderParser.Setup(x => x.TryParseOrder(lines[1], out order2))
            .Returns(true);

        // Act
        var result = _orderRepository.GetOrders(lines);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(order1, result);
        Assert.Contains(order2, result);
    }
    
    [Fact]
    public void GetOrders_WhenLinesAreInvalid_ShouldReturnEmptyList()
    {
        // Arrange
        var lines = new[] { "InvalidData1", "InvalidData2" };
        Order? nullOrder = null;

        _mockOrderParser
            .Setup(x => x.TryParseOrder(It.IsAny<string>(), out nullOrder))
            .Returns(false);

        // Act
        var result = _orderRepository.GetOrders(lines);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetOrders_WhenSomeLinesAreInvalid_ShouldReturnOnlyValidOrders()
    {
        // Arrange
        var lines = new[] { "1;2.5;DistrictA;2024-10-01 10:00:00", "InvalidData" };
        var validOrder = new Order { Id = 1, District = "DistrictA", DeliveryTime = DateTime.Parse("2024-10-01 10:00:00") };
        Order? nullOrder = null;

        _mockOrderParser
            .Setup(x => x.TryParseOrder(lines[0], out validOrder))
            .Returns(true);
        
        _mockOrderParser
            .Setup(x => x.TryParseOrder(lines[1], out nullOrder))
            .Returns(false);

        // Act
        var result = _orderRepository.GetOrders(lines);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(validOrder, result);
    }
    
    [Fact]
    public void FilterOrdersByDistrictAndTime_WhenOrdersMatchCriteria_ShouldReturnFilteredOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, District = "DistrictA", DeliveryTime = DateTime.Parse("2024-10-01 10:00:00") },
            new Order { Id = 2, District = "DistrictA", DeliveryTime = DateTime.Parse("2024-10-01 10:10:00") },
            new Order { Id = 3, District = "DistrictB", DeliveryTime = DateTime.Parse("2024-10-01 10:40:00") }
        };

        const string district = "DistrictA";
        var firstDeliveryTime = DateTime.Parse("2024-10-01 10:00:00");
        var interval = TimeSpan.FromMinutes(30);

        // Act
        var result = _orderRepository.FilterOrdersByDistrictAndTime(orders, district, firstDeliveryTime, interval);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal(district, o.District));
        Assert.All(result, o => Assert.True(o.DeliveryTime >= firstDeliveryTime && o.DeliveryTime <= firstDeliveryTime.Add(interval)));
    }
    
    [Fact]
    public void FilterOrdersByDistrictAndTime_WhenNoOrdersMatchFilter_ShouldReturnEmptyList()
    {
        // Arrange
        var orders = new List<Order>
        {
            new()
            {
                Id = 1,
                District = "DistrictA",
                DeliveryTime = DateTime.Parse("2024-10-01 10:00:00")
            },
            
            new()
            {
                Id = 2,
                District = "DistrictA",
                DeliveryTime = DateTime.Parse("2024-10-01 10:10:00")
            }
        };

        const string district = "DistrictB";
        var firstDeliveryTime = DateTime.Parse("2024-10-01 10:00:00");
        var interval = TimeSpan.FromMinutes(30);

        // Act
        var result = _orderRepository.FilterOrdersByDistrictAndTime(orders, district, firstDeliveryTime, interval);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}