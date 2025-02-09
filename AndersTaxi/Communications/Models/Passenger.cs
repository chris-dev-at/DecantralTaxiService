namespace Communications.Models;

public class Passenger
{
    public string Id { get; set; }
    public double MaxPricePerKm { get; set; }
    
    public override string ToString() => $"Id: {Id}, MaxPricePerKm: {MaxPricePerKm}";
}