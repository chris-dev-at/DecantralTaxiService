using Communications;
using Communications.Models;

namespace Simulator.Components.Models;

public class SimDriver : Driver, IDisposable
{
    
    private CancellationTokenSource _cts = new();
    private Random _random = new();
    private HttpClient _client;
    
    public SimDriver(string id, Location location, double pricePerKm, HttpClient client)
    {
        Id = id;
        State = DriverState.Available;
        Location = location;
        PricePerKm = pricePerKm;
        
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
                await Task.Delay(1000);
            }
        }, token);
    }
    
    public async Task Tick()
    {
        //Default Behavior of driver device
        await UpdateLocation();
        
        
        
        //Actions by driver (simulated)
        //move driver randomly
        var move = _random.Next(0, 5);
        switch (move)
        {
            case 0:
                MoveUp();
                break;
            case 1:
                MoveDown();
                break;
            case 2:
                MoveLeft();
                break;
            case 3:
                MoveRight();
                break;
            case 4:
                //Do nothing
                break;
        }
    }
    
    public void MoveUp()
    {
        if(Location.Y >= GlobalConfig.GlobalWidth-1)
            return;
        Location.Y++;
    }
    public void MoveDown()
    {
        if(Location.Y <= 0)
            return;
        Location.Y--;
    }
    public void MoveLeft()
    {
        if(Location.X <= 0)
            return;
        Location.X--;
    }
    public void MoveRight()
    {
        if(Location.X >= GlobalConfig.GlobalLength-1)
            return;
        Location.X++;
    }
    
    
    private async Task UpdateLocation() =>
        await _client.GetAsync($"http://driverservice:8080/updateLocation?id={Id}&x={Location.X}&y={Location.Y}&state={State}");
    

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}