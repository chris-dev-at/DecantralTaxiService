# DecantralTaxiService
A decentralized taxi service based on asynchronous communication and message-oriented middleware
# Configuration
Configuration is located in `AndersTaxi/Communications/GlobalConfig.cs`.
```csharp
public static class GlobalConfig
{
    public static string HostName = "rabbitmq";
    public static int GlobalLength = 30;
    public static int GlobalWidth = 30;
    public static int TimeBetweenTicksMs = 1500;
    public static int MaxAcceptableDistance = 10;
    public static double MaxPricePerKm = 0.8;
    public static double MinPricePerKm = 0.2;
}
```
To apply the configuration, a restart is required!


# Systemplan
![Systemplan](https://github.com/user-attachments/assets/06b758eb-3dad-4166-849b-cc940d882d22)
