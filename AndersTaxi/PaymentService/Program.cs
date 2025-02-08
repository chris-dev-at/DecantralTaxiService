using Communications;
using Communications.ExchangeMessages;

namespace PaymentService;

class Program
{
    static async Task Main(string[] args)
    {
        ManualResetEvent _quitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, eArgs) =>
        {
            _quitEvent.Set();
            eArgs.Cancel = true;
        };

        //Code
        var producerHandler = await (await CommunicationHandlerFactory.Initialize("rabbitmq")).CreateProducerHandler("taxi.topic");
        var locationSystem = new PaymentSystem(producerHandler);
        
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("location_queue");

        consumerHandler.OnMessage += locationSystem.Consume!;

        
        
        //keep alive
        _quitEvent.WaitOne();
    }
    
    public class PaymentSystem
    {
        public ProducerHandler ProducerHandler { get; set; }

        public PaymentSystem(ProducerHandler producerHandler)
        {
            this.ProducerHandler = producerHandler;
        }
        
        public void Consume(object sender, MessageArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.RideComplete:
                    var rideCompleteMessage = e.Message as RideCompleteMessage;
                    Console.WriteLine($"Ride Completed: {rideCompleteMessage?.Ride.PassengerId}, {rideCompleteMessage?.Ride.DriverId} for driver {rideCompleteMessage?.Ride.Distance}");
                    RideCompleteCall(rideCompleteMessage!);
                    break;
                default:
                    Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
                    break;
            }
        }
        
        public async Task RideCompleteCall (RideCompleteMessage message)
        {
            var invoice = new InvoiceEnquiryMessage()
            {
                Amount = message.Ride.Distance * message.Ride.PricePerKm,
            };
            await ProducerHandler.SendMessageAsync("payment", invoice);
        }
    }
}