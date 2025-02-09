using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

namespace Simulator.Components.Models;

public class LocationSystemSimulated : IDisposable
{
    public List<LocationEntry> Drivers { get; set; }

    private CancellationTokenSource _cts = new();
    
    public object LockObject = new object();

    public LocationSystemSimulated()
    {
        this.Drivers = new List<LocationEntry>();
        
        StartAutoUpdate(_cts.Token);
    }
    
    private async Task StartAutoUpdate(CancellationToken token)
    {
        //Dont await here, else page will load forever
        await Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                ClearOldEntries();
                await Task.Delay(1000);
            }
        }, token);
    }
    
    public void UpdateLocationCall(LocationUpdateMessage message)
    {
        if(message.Driver.Location.X > GlobalConfig.GlobalLength || message.Driver.Location.Y > GlobalConfig.GlobalWidth)
        {
            Console.WriteLine("Invalid location update");
            return;
        }
        lock (LockObject)
        {
            this.Drivers.RemoveAll(le => le.Driver.Id == message.Driver.Id);
            this.Drivers.Add(new LocationEntry()
            {
                Driver = message.Driver,
                Time = DateTime.Now
            }); 
        }
    }

    public async Task RequestRideCall(RequestRideMessage message)
    {
        //get all drivers in range of GlobalConfig.MaxAcceptableDistance and available
        var driversInRange = this.Drivers.Where(x => 
                x.Driver.State == DriverState.Available && 
                x.Driver.PricePerKm <= message.Ride.Passenger.MaxPricePerKm &&
                x.Driver.Location.Distance(message.Ride.StartLocation) < GlobalConfig.MaxAcceptableDistance)
            .ToList();
        
        //Do nothing for now
    }

    public void ClearOldEntries() //In case of a driver disconnecting
    {
        //check if Drivers is locked
        lock (LockObject)
        {
            this.Drivers.RemoveAll(x => x.Time < DateTime.Now.AddSeconds(-5)); //Remove all entries older than 5 seconds
        }
    }
    
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}