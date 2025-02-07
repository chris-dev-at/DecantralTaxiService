using Communications.Models;

namespace Communications.ExchangeMessages;

public class RequestRideMessage : ExchangeMessage
{
    public Ride Ride { get; set; }
}