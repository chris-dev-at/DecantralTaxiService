namespace Communications.Models;

public class Location
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public static int Distance(Location a, Location b) //works for now but should be replaced with a better algorithm
    {
        return Convert.ToInt32(Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
    }
    public int Distance(Location other)
    {
        return Distance(this, other);
    }
}