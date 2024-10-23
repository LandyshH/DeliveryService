using Infrastructure.Configurations;

namespace Delivery.Services;

public interface IParametersService
{
    (string cityDistrict, string deliveryLogPath, string deliveryOrderPath, DateTime firstDeliveryDateTime)?
        LoadAndValidateParameters(OrderOptions? orderConfig, string[] args, string baseDirectory);
}