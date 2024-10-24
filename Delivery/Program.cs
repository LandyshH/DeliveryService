using System.Text.RegularExpressions;
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
            
            Log.Information("Приложение запущено.");

            var configuration = LoadConfiguration(baseDirectory);

            var orderConfig = configuration.GetSection("Order").Exists()
                ? configuration.GetRequiredSection("Order").Get<OrderOptions>()
                : null; 

            var parametersService = serviceProvider.GetService<IParametersService>();
            
            if (args.Length == 0)
            {
                Console.WriteLine("Введите параметры, либо нажмите Enter для загрузки из конфигурационного файла. \n Формат параметров: \n _cityDistrict \"<Название района>\" _firstDeliveryDateTime \"<Дата и время первой доставки в формате yyyy-MM-dd HH:mm:ss>\" _deliveryLog \"<Путь к файлу лога>\" _deliveryOrder \"<Путь к файлу заказов>");

                var userInput = Console.ReadLine();

                if (userInput != null) args = ParseUserInput(userInput);
            }
            
            Log.Information("Получение параметров.");
            
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

            Log.Information($"Логи записаны в файл: {deliveryLogPath}");
            Log.Information("Завершение программы.");

            Console.WriteLine("Нажмите любую клавишу для выхода из программы.");
            Console.ReadLine();
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
    
    private static string[] ParseUserInput(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return [];
        }

        var matches = Regex.Matches(userInput, @"(\w+)\s+""([^""]*)""|(\w+)\s+(\S+)");
        var argsList = new List<string>();

        foreach (Match match in matches)
        {
            if (match.Groups[2].Success)
            {
                argsList.Add(match.Groups[1].Value);
                argsList.Add(match.Groups[2].Value);
            }
            else
            {
                argsList.Add(match.Groups[3].Value);
                argsList.Add(match.Groups[4].Value);
            }
        }

        return argsList.ToArray();
    }
}