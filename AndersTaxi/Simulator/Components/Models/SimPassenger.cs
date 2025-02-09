using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

namespace Simulator.Components.Models;

public class SimPassenger : Passenger, IDisposable
{
    private CancellationTokenSource _cts = new();
    private Random _random = new();
    private HttpClient _client;
    
    public Ride CurrentRide;
    public bool StartedOnce = false;
    
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


        RequestRide();

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


    public async Task RequestRide()
    {
        var response = await _client.GetAsync($"http://passengerservice:8080/requestRide?id={Id}&x={CurrentRide.StartLocation.X}&y={CurrentRide.StartLocation.Y}&destinationX={CurrentRide.EndLocation.X}&destinationY={CurrentRide.EndLocation.Y}&maxPrice={MaxPricePerKm}");
    }
    
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
