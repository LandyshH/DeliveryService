using Core.Files;
using Core.Orders;
using Moq;

namespace Tests.Core.Orders
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IFileReader> _fileReaderMock;
        private readonly Mock<IFileWriter> _fileWriterMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _fileReaderMock = new Mock<IFileReader>();
            _fileWriterMock = new Mock<IFileWriter>();
            _orderService =
                new OrderService(_orderRepositoryMock.Object, _fileReaderMock.Object, _fileWriterMock.Object);
        }

        private const string FakeBaseDirectory = "directory";

        [Fact]
        public void ProcessOrders_WhenOrdersExistAndValid_ShouldFilterAndWriteToFile()
        {
            // Arrange
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var ordersFilePath = Path.Combine(baseDirectory, "Data", "orders_data.txt");
            const string deliveryOrderPath = "output.txt";
            const string cityDistrict = "DistrictB";
            var firstDeliveryDateTime = DateTime.Parse("2024-10-01 10:00:00");

            var ordersLines = new[] { "1;2.5;DistrictB;2024-10-01 10:00:00", "2;1.0;DistrictB;2024-10-01 10:15:00" };
            var orders = new List<Order>
            {
                new()
                {
                    Id = 1,
                    District = "DistrictB",
                    DeliveryTime = DateTime.Parse("2024-10-01 10:00:00")
                },
                new()
                {
                    Id = 2,
                    District = "DistrictA",
                    DeliveryTime = DateTime.Parse("2024-10-01 10:15:00")
                }
            };

            var filteredOrders = new List<Order>
            {
                new()
                {
                    Id = 1,
                    District = "DistrictB",
                    DeliveryTime = DateTime.Parse("2024-10-01 10:00:00")
                }
            };

            _fileReaderMock
                .Setup(x => x.ReadFileText(ordersFilePath))
                .Returns(ordersLines);

            _orderRepositoryMock
                .Setup(x => x.GetOrders(ordersLines))
                .Returns(orders);

            _orderRepositoryMock
                .Setup(x => x.FilterOrdersByDistrictAndTime(orders, cityDistrict, firstDeliveryDateTime,
                    It.IsAny<TimeSpan>()))
                .Returns(filteredOrders);

            // Act
            _orderService.ProcessOrders(baseDirectory, cityDistrict, firstDeliveryDateTime, deliveryOrderPath);

            // Assert
            _fileReaderMock.Verify(x => x.ReadFileText(ordersFilePath), Times.Once);
            _orderRepositoryMock.Verify(x => x.GetOrders(ordersLines), Times.Once);
            _orderRepositoryMock.Verify(
                x => x.FilterOrdersByDistrictAndTime(orders, cityDistrict, firstDeliveryDateTime, It.IsAny<TimeSpan>()),
                Times.Once);
            _fileWriterMock.Verify(x => x.WriteItemsListToFile(deliveryOrderPath, filteredOrders), Times.Once);
        }

        [Fact]
        public void ProcessOrders_WhenNoOrdersToProcess_ShouldReturn()
        {
            // Arrange
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            const string deliveryOrderPath = "output.txt";
            const string cityDistrict = "DistrictB";
            var firstDeliveryDateTime = DateTime.Parse("2024-10-01 10:00:00");
            var emptyOrdersLines = Array.Empty<string>();

            _fileReaderMock
                .Setup(fr => fr.ReadFileText(It.IsAny<string>()))
                .Returns([]);

            _orderRepositoryMock
                .Setup(x => x.GetOrders(emptyOrdersLines))
                .Returns([]);

            // Act
            _orderService.ProcessOrders(baseDirectory, cityDistrict, firstDeliveryDateTime, deliveryOrderPath);

            // Assert
            _orderRepositoryMock.Verify(or => or.GetOrders(It.IsAny<string[]>()), Times.Once);
            _orderRepositoryMock.Verify(
                or => or.FilterOrdersByDistrictAndTime(It.IsAny<List<Order>>(), It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<TimeSpan>()), Times.Never);
            _fileWriterMock.Verify(fw => fw.WriteItemsListToFile(It.IsAny<string>(), It.IsAny<List<Order>>()),
                Times.Never);
        }

        [Fact]
        public void ProcessOrders_WhenOrdersFileNotFound_ShouldReturn()
        {
            // Arrange
            const string baseDirectory = "directory";
            const string deliveryOrderPath = "output.txt";
            const string cityDistrict = "DistrictB";
            var firstDeliveryDateTime = DateTime.Parse("2024-10-01 10:00:00");

            _fileReaderMock
                .Setup(fr => fr.ReadFileText(It.IsAny<string>()))
                .Returns([]);

            // Act
            _orderService.ProcessOrders(baseDirectory, cityDistrict, firstDeliveryDateTime, deliveryOrderPath);

            // Assert
            _fileReaderMock.Verify(fr => fr.ReadFileText(It.IsAny<string>()), Times.Never);
            _orderRepositoryMock.Verify(or => or.GetOrders(It.IsAny<string[]>()), Times.Never);
            _orderRepositoryMock.Verify(
                or => or.FilterOrdersByDistrictAndTime(It.IsAny<List<Order>>(), It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<TimeSpan>()), Times.Never);
            _fileWriterMock.Verify(fw => fw.WriteItemsListToFile(It.IsAny<string>(), It.IsAny<List<Order>>()),
                Times.Never);
        }

        [Fact]
        public void ProcessOrders_NoFilteredOrders_ShouldReturn()
        {
            // Arrange
            const string baseDirectory = "directory";
            const string deliveryOrderPath = "output.txt";
            const string cityDistrict = "DistrictB";
            var firstDeliveryDateTime = DateTime.Parse("2024-10-01 10:00:00");
            var ordersLines = new[] { "1;2.5;DistrictB;2024-10-01 10:00:00", "2;1.0;DistrictB;2024-10-01 10:15:00" };

            var orders = new List<Order>
            {
                new()
                {
                    Id = 1,
                    District = "DistrictB",
                    DeliveryTime = DateTime.Parse("2024-10-01 10:00:00")
                },
                new()
                {
                    Id = 2,
                    District = "DistrictA",
                    DeliveryTime = DateTime.Parse("2024-10-01 10:15:00")
                }
            };

            var filteredOrders = new List<Order>();

            _fileReaderMock
                .Setup(fr => fr.ReadFileText(It.IsAny<string>()))
                .Returns(ordersLines);

            _orderRepositoryMock
                .Setup(or => or.GetOrders(ordersLines))
                .Returns(orders);

            _orderRepositoryMock
                .Setup(or =>
                    or.FilterOrdersByDistrictAndTime(orders, cityDistrict, firstDeliveryDateTime,
                        TimeSpan.FromMinutes(30)))
                .Returns(filteredOrders);

            // Act
            _orderService.ProcessOrders(baseDirectory, cityDistrict, firstDeliveryDateTime, deliveryOrderPath);

            // Assert
            _fileWriterMock.Verify(fw => fw.WriteItemsListToFile(It.IsAny<string>(), It.IsAny<List<Order>>()),
                Times.Never);
        }
    }
}