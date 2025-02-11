using System.Net;
using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

namespace Simulator.Components.Models;

public class SimDriver : Driver, IDisposable
{
    
    private CancellationTokenSource _cts = new();
    private Random _random = new();
    private HttpClient _client;
    
    public Ride CurrentRide { get; set; }
    
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
        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Tick();
                await Task.Delay(GlobalConfig.TimeBetweenTicksMs);
            }
        }, token);
    }
    
    public async Task Tick()
    {
        //Default Behavior of driver device
        await UpdateLocation();
        
        //Actions by driver (simulated)
        switch (this.State)
        {
            case DriverState.Unavailable:
                await Task.Delay(GlobalConfig.TimeBetweenTicksMs*4);
                State = DriverState.Available;
                break;
            case DriverState.Available:
                MoveRandom();
                await CheckRides();
                break;
            case DriverState.OfferedRide:
                //Do nothing (no longer in use)
                break;
            case DriverState.OnRouteToPassenger:
                var arrived1 = MoveTowards(CurrentRide.StartLocation);
                if (arrived1)
                {
                    State = DriverState.OnRouteToDestination;
                }
                break;
            case DriverState.OnRouteToDestination:
                var arrived2 = MoveTowards(CurrentRide.EndLocation);
                if (arrived2)
                {
                    await CompleteRide();
                    State = DriverState.Unavailable;
                }
                break;
        }
    }


    public async Task CompleteRide()
    {
        var res = await _client.GetAsync($"http://driverservice:8080/completeRide?passengerId={CurrentRide.Passenger.Id}&driverId={Id}&distance={CurrentRide.Distance}&pricePerKm={PricePerKm}");

        CurrentRide = null;
        State = DriverState.Available;
    }
    
    public async Task CheckRides()
    {
        //get all open requests
        var openRequests = await _client.GetFromJsonAsync<List<RequestDriverMessage>>($"http://driverservice:8080/getOpenRequests?id={Id}");
        
        //For now 100%  --(90% Chance to accept a ride) 
        if (openRequests.Count > 0 /*&& _random.Next(0, 100) < 90*/)
        {
            var request = openRequests.First();

            //check if driver likes the price
            if (request.Ride.Passenger.MaxPricePerKm < PricePerKm)
            {
                return;
            }
            
            var res = await _client.GetAsync($"http://driverservice:8080/acceptRequest?driverId={Id}&passengerId={request.Ride.Passenger.Id}");
            if (res.StatusCode == HttpStatusCode.OK)
            {
                CurrentRide = request.Ride; 
                State = DriverState.OnRouteToPassenger;
            }
            if(res.StatusCode == HttpStatusCode.NotFound)
                State = DriverState.Available;
        }
    }
    
    
    //return true if driver is at location
    public bool MoveTowards(Location location)
    {
        if (CurrentRide != null)
            CurrentRide.Distance++;
        
        if (Location.X < location.X)
        {
            MoveRight();
        }
        else if (Location.X > location.X)
        {
            MoveLeft();
        }
        else if (Location.Y < location.Y)
        {
            MoveUp();
        }
        else if (Location.Y > location.Y)
        {
            MoveDown();
        }
        return Location.X == location.X && Location.Y == location.Y;
    }
    public void MoveRandom()
    {
        //move driver randomly
        var move = _random.Next(0, 100);
        if (move < 20)
        {
            MoveUp();
        }
        else if (move < 40)
        {
            MoveDown();
        }
        else if (move < 60)
        {
            MoveLeft();
        }
        else if (move < 80)
        {
            MoveRight();
        }
        else if (move < 97)
        {
            //Do nothing
        }
        else
        {
            State = DriverState.Unavailable; //somehow crashes acceptance of rides somehow (not sure why and wont fix it unless needed) ;)
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