using Communications;
using Communications.Models;

namespace Simulator.Components.Models;

public class SimPassenger : Passenger, IDisposable
{
    private CancellationTokenSource _cts = new();
    private Random _random = new();
    private HttpClient _client;
    
    public Ride CurrentRide;
    
    public SimPassenger(string id, Location startLocation, Location destination, double maxPrice, HttpClient client)
    {
        Id = id;
        MaxPricePerKm = maxPrice;
        
        CurrentRide = new Ride()
        {
            Passenger = this,
            StartLocation = startLocation,
            EndLocation = destination,
        };
        
        _client = client;
        
        StartAutoUpdate(_cts.Token);
    }
    
    private async Task StartAutoUpdate(CancellationToken token)
    {
        await Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                Tick();
                await Task.Delay(GlobalConfig.TimeBetweenTicksMs);
            }
        }, token);
    }

    public async Task Tick()
    {
    }
    


    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
