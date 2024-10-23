using Serilog;

namespace Infrastructure.Configurations;

public static class LogConfiguration
{
    public static void SetupLogging(string deliveryLogPath)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(deliveryLogPath)
            .CreateLogger();
    }
    
    public static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
    }
}