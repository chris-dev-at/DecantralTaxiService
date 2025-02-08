namespace Communications.Models;

public class Driver
{
    public string Id { get; set; }
    public DriverState State { get; set; }
    public Location Location { get; set; }
    public double PricePerKm { get; set; }

    public override string ToString() => $"Id: {Id}, State: {State}, Position: {Location}";
}