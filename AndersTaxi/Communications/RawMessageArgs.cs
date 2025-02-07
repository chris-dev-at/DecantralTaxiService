using RabbitMQ.Client.Events;

namespace Communications;

public class RawMessageArgs
{
    public string Message { get; }
    public BasicDeliverEventArgs BasicDeliverEventArgs { get; }
    
    public RawMessageArgs(string message, BasicDeliverEventArgs bde)
    {
        Message = message;
        BasicDeliverEventArgs = bde;
    }
}