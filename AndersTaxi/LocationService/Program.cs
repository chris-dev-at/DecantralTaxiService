using Communications;
using LocationService.Models;

namespace LocationService;

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
        var locationSystem = new LocationSystem(producerHandler);
        
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("location_queue");

        consumerHandler.OnMessage += locationSystem.Consume!;
        
        //keep alive
        _quitEvent.WaitOne();
    }
}