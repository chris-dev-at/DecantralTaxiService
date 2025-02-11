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
    public int TotalDistanceDriven = 0;
    public double TotalRevenue = 0;
    public int TotalRides = 0;

    public List<SimDriver> SimDrivers = new List<SimDriver>();
    public object SimPassemgerLock = new object();
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
            case MessageType.InvoiceEnquiry:
                var invoiceEnquiryMessage = e.Message as InvoiceEnquiryMessage;
                //Console.WriteLine($"Invoice Enquiry: {invoiceEnquiryMessage?.PassengerId} wants to know the price of a ride");
                InvoiceEnquiryCall(invoiceEnquiryMessage!);
                break;
            case MessageType.RideComplete:
                var rideCompleteMessage = e.Message as RideCompleteMessage;
                RideCompleteCall(rideCompleteMessage!);
                break;
            default:
                //Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
                break;
        }
    }
    
    private void InvoiceEnquiryCall(InvoiceEnquiryMessage message) => this.TotalRevenue += message.Amount; 
    private void RideCompleteCall(RideCompleteMessage message)
    {
        this.TotalDistanceDriven += message.Ride.Distance;
        this.TotalRides++;
    }     
        
    
    
}