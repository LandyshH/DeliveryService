using Core.Files;
using Core.Orders;
using Delivery.Services;
using Infrastructure.Configurations;
using Infrastructure.Files;
using Infrastructure.Orders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Delivery;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IOrderParser, OrderParser>()
                .AddSingleton<IOrderRepository, OrderRepository>()
                .AddSingleton<IFileReader, FileReader>()
                .AddSingleton<IFileWriter, FileWriter>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IParametersService, ParametersService>()
                .BuildServiceProvider();

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            LogConfiguration.SetupLogging();

            var configuration = LoadConfiguration(baseDirectory);

            var orderConfig = configuration.GetSection("Order").Exists()
                ? configuration.GetRequiredSection("Order").Get<OrderOptions>()
                : null; 

            var parametersService = serviceProvider.GetService<IParametersService>();
            var parameters = parametersService!.LoadAndValidateParameters(orderConfig, args, baseDirectory);

            if (parameters == null)
            {
                Log.Error("Все или некоторые параметры отсутствуют.");
                return;
            }

            var (cityDistrict, deliveryLogPath, deliveryOrderPath, firstDeliveryDateTime) = parameters.Value;

            LogConfiguration.SetupLogging(deliveryLogPath);

            Log.Information(
                $"Запуск приложения с параметрами: район доставки = {cityDistrict}, время первой доставки = {firstDeliveryDateTime}");

            var orderService = serviceProvider.GetService<IOrderService>();
            orderService!.ProcessOrders(baseDirectory, cityDistrict, firstDeliveryDateTime, deliveryOrderPath);

            Log.Information("Завершение программы.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Произошла критическая ошибка. Завершение программы.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IConfiguration LoadConfiguration(string baseDirectory)
    {
        return new ConfigurationBuilder()
            .SetBasePath(baseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}