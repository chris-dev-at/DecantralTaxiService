# DecantralTaxiService
A decentralized taxi service based on asynchronous communication and message-oriented middleware

# Systemplan
![Systemplan](https://github.com/user-attachments/assets/06b758eb-3dad-4166-849b-cc940d882d22)

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

# Preview
This is a taxi service simulation application that demonstrates ride-hailing operations with drivers and passengers. The system consists of five microservices handling different aspects of the taxi service and one simulation app for testing interactions.

![image](https://github.com/user-attachments/assets/57a1c322-ef65-4789-b114-2f4acf84f01f)


