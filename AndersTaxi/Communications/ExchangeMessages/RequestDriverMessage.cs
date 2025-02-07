using Communications.Models;

namespace Communications.ExchangeMessages;

public class RequestDriverMessage : ExchangeMessage
{
    public Ride Ride { get; set; }
    public List<string> DriverIds { get; set; } //potential drivers
    
    public RequestDriverMessage()
    {
        Type = MessageType.RequestDriver;
    }
}