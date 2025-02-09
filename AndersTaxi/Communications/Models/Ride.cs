namespace Communications.Models;

public class Ride
{
    public Passenger Passenger { get; set; }
    public Driver Driver { get; set; }
    public Location StartLocation { get; set; }
    public Location EndLocation { get; set; }
    public int Distance { get; set; }
    
    public override string ToString() => $"Passenger: {Passenger.Id}, Driver: {Driver.Id}, Start: {StartLocation}, End: {EndLocation}, Distance: {Distance}, PricePerKm: {Driver.PricePerKm}";
}