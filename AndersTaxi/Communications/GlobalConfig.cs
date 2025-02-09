namespace Communications;

public static class GlobalConfig
{
    public static string HostName = "rabbitmq";
    public static int GlobalLength = 30;
    public static int GlobalWidth = 30;
    public static int TimeBetweenTicksMs = 1000;
    public static int MaxAcceptableDistance = 20;
    public static double MaxPricePerKm = 0.8;
    public static double MinPricePerKm = 0.2;
}