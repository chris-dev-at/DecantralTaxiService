namespace Communications.Models;

public class Ride
{
    public string PassengerId { get; set; }
    public Driver Driver { get; set; }
    public Location StartLocation { get; set; }
    public Location EndLocation { get; set; }
    public int Distance { get; set; }
    
    public override string ToString() => $"Passenger: {PassengerId}, Driver: {Driver.Id}, Start: {StartLocation}, End: {EndLocation}, Distance: {Distance}, PricePerKm: {Driver.PricePerKm}";
}