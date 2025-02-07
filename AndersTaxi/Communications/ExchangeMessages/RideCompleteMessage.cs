using Communications.Models;

namespace Communications.ExchangeMessages;

public class RideCompleteMessage : ExchangeMessage
{
    public Ride Ride { get; set; }
}