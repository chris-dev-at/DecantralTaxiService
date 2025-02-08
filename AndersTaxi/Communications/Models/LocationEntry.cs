namespace Communications.Models;

public class LocationEntry //Driver Location Time
{
    public Driver Driver { get; set; }
    public DateTime Time { get; set; }
    
    public override string ToString() => $"Time: {Time}, Driver: {Driver}";
}