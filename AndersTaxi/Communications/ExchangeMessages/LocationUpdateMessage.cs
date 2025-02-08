using Communications.Models;

namespace Communications.ExchangeMessages;

public class LocationUpdateMessage : ExchangeMessage
{
    public Driver Driver { get; set; }
    
    public LocationUpdateMessage()
    {
        Type = MessageType.LocationUpdate;
    }
}