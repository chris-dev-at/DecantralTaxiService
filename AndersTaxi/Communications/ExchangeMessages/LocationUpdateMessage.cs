using Communications.Models;

namespace Communications.ExchangeMessages;

public class LocationUpdateMessage : ExchangeMessage
{
    public Location Location { get; set; }
    public string DriverId { get; set; }
}