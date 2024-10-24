using System.Globalization;
using Infrastructure.Configurations;
using Serilog;

namespace Delivery.Services;

public class ParametersService : IParametersService
{
    public (string cityDistrict, string deliveryLogPath, string deliveryOrderPath, DateTime firstDeliveryDateTime)? 
        LoadAndValidateParameters(OrderOptions? orderConfig, string[] args, string baseDirectory)
    {
        var (cityDistrict, deliveryLogPath, deliveryOrderPath, firstDeliveryDateTimeStr) = LoadParameters(orderConfig, args, baseDirectory);
        
        if (!ValidateParameters(cityDistrict, firstDeliveryDateTimeStr, deliveryLogPath, deliveryOrderPath))
        {
            return null; 
        }

        if (!TryParseDeliveryDateTime(firstDeliveryDateTimeStr, out var firstDeliveryDateTime))
        {
            return null; 
        }

        return (cityDistrict, deliveryLogPath, deliveryOrderPath, firstDeliveryDateTime);
    }
    
    private static (string cityDistrict, string deliveryLogPath, string deliveryOrderPath, string firstDeliveryDateTimeStr) 
        LoadParameters(OrderOptions? orderConfig, string[] args, string baseDirectory)
    {
        
        var cityDistrict = string.Empty;
        var deliveryLogPath = string.Empty;
        var deliveryOrderPath = string.Empty;
        var firstDeliveryDateTimeStr = string.Empty;
        
        if (orderConfig is not null)
        {
            cityDistrict = orderConfig.CityDistrict;
            
            deliveryLogPath = orderConfig.DeliveryLog is not null 
                ? Path.Combine(baseDirectory, orderConfig.DeliveryLog) 
                : string.Empty;
            
            deliveryOrderPath = orderConfig.DeliveryOrder is not null 
                ? Path.Combine(baseDirectory, orderConfig.DeliveryOrder) 
                : string.Empty;
            
            firstDeliveryDateTimeStr = orderConfig.FirstDeliveryDateTime;
        }

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "_cityDistrict":
                    if (i + 1 < args.Length)
                    {
                        cityDistrict = args[i + 1].Trim('"');
                        i++;
                    }
                    break;

                case "_firstDeliveryDateTime":
                    if (i + 1 < args.Length)
                    {
                        firstDeliveryDateTimeStr = args[i + 1].Trim('"');
                        i++;
                    }
                    break;

                case "_deliveryLog":
                    if (i + 1 < args.Length)
                    {
                        deliveryLogPath = args[i + 1].Trim('"');
                        i++;
                    }
                    break;

                case "_deliveryOrder":
                    if (i + 1 < args.Length)
                    {
                        deliveryOrderPath = args[i + 1].Trim('"');
                        i++;
                    }
                    break;
            }
        }

        return (cityDistrict, deliveryLogPath, deliveryOrderPath, firstDeliveryDateTimeStr);
    }
    
    private static bool ValidateParameters(string cityDistrict, string firstDeliveryDateTimeStr, string deliveryLogPath, string deliveryOrderPath)
    {
        if (string.IsNullOrWhiteSpace(cityDistrict) || string.IsNullOrWhiteSpace(firstDeliveryDateTimeStr) ||
            string.IsNullOrWhiteSpace(deliveryLogPath) || string.IsNullOrWhiteSpace(deliveryOrderPath))
        {
            return false;
        }
        
        return true;
    }

    private static bool TryParseDeliveryDateTime(string firstDeliveryDateTimeStr, out DateTime firstDeliveryDateTime)
    {
        if (DateTime.TryParseExact(
                firstDeliveryDateTimeStr,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out firstDeliveryDateTime))
        {
            return true;
        }
        
        Log.Error("Некорректное значение времени первой доставки.");
        
        return false;
    }
}