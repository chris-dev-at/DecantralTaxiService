using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

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
        var producerHandler = await (await CommunicationHandlerFactory.Initialize("rabbitmq")).CreateProducerHandler("taxi.topic");
        var locationSystem = new LocationSystem(producerHandler);
        
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("location_queue");

        consumerHandler.OnMessage += locationSystem.Consume!;

        
        
        //keep alive
        _quitEvent.WaitOne();
    }
    
    

    public class LocationSystem
    {
        public List<LocationEntry> Drivers { get; set; }
        public ProducerHandler ProducerHandler { get; set; }

        public LocationSystem(ProducerHandler producerHandler)
        {
            this.Drivers = new List<LocationEntry>();
            this.ProducerHandler = producerHandler;
        }
        
        public void Consume(object sender, MessageArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.LocationUpdate:
                    var locationMessage = e.Message as LocationUpdateMessage;
                    Console.WriteLine($"Location Update: {locationMessage?.Driver.Location.X}, {locationMessage?.Driver.Location.Y} for driver {locationMessage?.Driver.Id}");
                    UpdateLocationCall(locationMessage!);
                    break;
                case MessageType.RequestRide:
                    var requestRideMessage = e.Message as RequestRideMessage;
                    Console.WriteLine($"Request Ride: {requestRideMessage?.Ride.PassengerId} wants a ride at {requestRideMessage?.Ride.StartLocation.X}, {requestRideMessage?.Ride.StartLocation.Y}");
                    RequestRideCall(requestRideMessage!);
                    break;
                default:
                    Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
                    break;
            }
        }
        
        public void UpdateLocationCall(LocationUpdateMessage message)
        {
            if(message.Driver.Location.X > GlobalConfig.GlobalLegth || message.Driver.Location.Y > GlobalConfig.GlobalWidth)
            {
                Console.WriteLine("Invalid location update");
                return;
            }
            
            this.Drivers.RemoveAll(le => le.Driver.Id == message.Driver.Id);
            this.Drivers.Add(new LocationEntry()
            {
                Driver = message.Driver,
                Time = DateTime.Now
            }); 
            
            ClearOldEntries(); 
        }

        public async Task RequestRideCall(RequestRideMessage message)
        {
            //get all drivers in range of GlobalConfig.MaxAcceptableDistance and available
            var driversInRange = this.Drivers.Where(x => 
                x.Driver.State == DriverState.Available && 
                x.Driver.Location.Distance(message.Ride.StartLocation) < GlobalConfig.MaxAcceptableDistance)
                .ToList();
            
            var requestDriverMessage = new RequestDriverMessage()
            {
                Ride = message.Ride,
                Drivers = driversInRange.Select(x => x.Driver.Id).ToList()
            };
            
            //Send message to DriverService
            await ProducerHandler.SendMessageAsync("driver", requestDriverMessage);
        }

        public void ClearOldEntries() //In case of a driver disconnecting
        {
            this.Drivers.RemoveAll(x => x.Time < DateTime.Now.AddSeconds(-5)); //Remove all entries older than 5 seconds
        }
        
    }
}