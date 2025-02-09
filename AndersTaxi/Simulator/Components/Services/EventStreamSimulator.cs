using Communications;
using Communications.ExchangeMessages;
using Communications.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Simulator.Components.Models;

namespace Simulator.Components.Services;

public class EventStreamSimulator
{
    private CommunicationHandlerFactory _communicationHandlerFactory;
    private ConsumerHandler _consumerHandler;
    
    public LocationSystemSimulated LocationSystem = new LocationSystemSimulated();

    public List<SimDriver> SimDrivers = new List<SimDriver>();
    public List<SimPassenger> SimPassengers = new List<SimPassenger>();
    
    public EventStreamSimulator(CommunicationHandlerFactory communicationHandlerFactory, ConsumerHandler consumerHandler)
    {
        _communicationHandlerFactory = communicationHandlerFactory;
        _consumerHandler = consumerHandler;
        
        _consumerHandler.OnMessage += Consume;
    }
    
    private void Consume(object sender, MessageArgs e)
    {
        switch (e.Message.Type)
        {
            case MessageType.LocationUpdate:
                var locationMessage = e.Message as LocationUpdateMessage;
                //Console.WriteLine($"Location Update: {locationMessage?.Driver.Location.X}, {locationMessage?.Driver.Location.Y} for driver {locationMessage?.Driver.Id}");
                LocationSystem.UpdateLocationCall(locationMessage!);
                break;
            case MessageType.RequestRide:
                var requestRideMessage = e.Message as RequestRideMessage;
                //Console.WriteLine($"Request Ride: {requestRideMessage?.Ride.PassengerId} wants a ride at {requestRideMessage?.Ride.StartLocation.X}, {requestRideMessage?.Ride.StartLocation.Y}");
                LocationSystem.RequestRideCall(requestRideMessage!);
                break;
            default:
                //Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
                break;
        }
    }
    
    
}