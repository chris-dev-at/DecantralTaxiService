using RabbitMQ.Client.Events;

namespace Communications;

public class MessageArgs
{
    public string Message { get; }
    public BasicDeliverEventArgs BasicDeliverEventArgs { get; }
    
    public MessageArgs(string message, BasicDeliverEventArgs bde)
    {
        Message = message;
        BasicDeliverEventArgs = bde;
    }
}