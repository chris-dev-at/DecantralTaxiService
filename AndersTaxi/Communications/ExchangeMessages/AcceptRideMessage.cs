using Communications.Models;

namespace Communications.ExchangeMessages;

public class AcceptRideMessage : ExchangeMessage
{
    public Ride Ride { get; set; }
    
    public AcceptRideMessage()
    {
        Type = MessageType.AcceptRide;
    }
}