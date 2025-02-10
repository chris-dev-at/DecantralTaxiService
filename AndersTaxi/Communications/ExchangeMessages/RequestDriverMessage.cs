using Communications.Models;

namespace Communications.ExchangeMessages;

public class RequestDriverMessage : ExchangeMessage
{
    public Ride Ride { get; set; }
    public List<string> Drivers { get; set; } //potential drivers
    public DateTime CreationTime { get; set; }
    
    public RequestDriverMessage()
    {
        Type = MessageType.RequestDriver;
    }
}