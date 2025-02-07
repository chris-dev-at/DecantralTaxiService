using Communications;
using Communications.ExchangeMessages;

namespace LocationService;

class Program
{
    static async Task Main(string[] args)
    {
        ManualResetEvent _quitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, eArgs) =>
        {
            _quitEvent.Set();
            eArgs.Cancel = true;
        };

        //Log for now to Console
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("location_queue");

        consumerHandler.OnMessage += Consume;

        //keep alive
        _quitEvent.WaitOne();
    }
    
    public static void Consume(object sender, MessageArgs e)
    {
        Console.WriteLine($"Processing Message: {e.Message.Type}");

        switch (e.Message.Type)
        {
            case MessageType.LocationUpdate:
                Console.WriteLine("Casting to LocationUpdateMessage");
                var locationMessage = e.Message as LocationUpdateMessage;
                Console.WriteLine($"Location Update: {locationMessage?.Location.X}, {locationMessage?.Location.Y} for driver {locationMessage?.DriverId}");
                break;
            default:
                Console.WriteLine("Unknown message type");
                break;
        }

        Console.WriteLine("Message processed");
    }
}