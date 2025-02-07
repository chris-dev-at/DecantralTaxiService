using Communications.Models;

namespace Communications.ExchangeMessages;

public class OfferRideMessage : ExchangeMessage
{
    public Ride Ride { get; set; }
    
    public OfferRideMessage()
    {
        Type = MessageType.OfferRide;
    }
}