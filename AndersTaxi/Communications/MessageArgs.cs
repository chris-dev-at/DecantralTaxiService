using RabbitMQ.Client.Events;
using Communications.ExchangeMessages;

namespace Communications;

public class MessageArgs
{
    public ExchangeMessage Message { get; }
    public BasicDeliverEventArgs BasicDeliverEventArgs { get; }
    
    public MessageArgs(ExchangeMessage message, BasicDeliverEventArgs bde)
    {
        Message = message;
        BasicDeliverEventArgs = bde;
    }
}