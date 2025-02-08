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

        //Code
        var locationSystem = new LocationSystem(GlobalConfig.GlobalLegth, GlobalConfig.GlobalWidth);
        
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("location_queue");

        consumerHandler.OnMessage += locationSystem.Consume!;

        
        
        //keep alive
        _quitEvent.WaitOne();
    }
    
    

    public class LocationSystem
    {
        public string[,] Grid { get; set; }

        public LocationSystem(int length, int width)
        {
            this.Grid = new string[length, width];
        }
        
        public void Consume(object sender, MessageArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.LocationUpdate:
                    var locationMessage = e.Message as LocationUpdateMessage;
                    Console.WriteLine($"Location Update: {locationMessage?.Location.X}, {locationMessage?.Location.Y} for driver {locationMessage?.DriverId}");
                    break;
                default:
                    Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
                    break;
            }
        }
        
        public void UpdateLocationCall(LocationUpdateMessage message)
        {
            Grid[message.Location.X, message.Location.Y] = message.DriverId;
        }
        
    }
}