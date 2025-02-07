namespace Communications.Models;

public class Ride
{
    public Location StartLocation { get; set; }
    public Location EndLocation { get; set; }
    public string PassengerId { get; set; }
    public string DriverId { get; set; }
    public int Distance { get; set; }
    public double PricePerKm { get; set; }
}