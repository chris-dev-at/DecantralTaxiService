using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Passenger Says Hello!");

app.MapGet("/publish/", async() =>
{
    var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
    var producerHandler = await f.CreateProducerHandler("taxi.topic");
    await producerHandler.SendMessageAsync("driver", "Hello, World!");
    return "Published message";
});

app.MapGet("/publish/location", async() =>
{
    var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
    var producerHandler = await f.CreateProducerHandler("taxi.topic");

    var msg = new LocationUpdateMessage()
    {
        Driver = new Driver()
        {
            Location = new Location() { X = new Random().Next(0, GlobalConfig.GlobalLength), Y = new Random().Next(0, GlobalConfig.GlobalWidth)},
            Id = $"hokuspokus{new Random().Next(0, 1000)}",
            State = DriverState.Available
        }
    };
    
    await producerHandler.SendMessageAsync("location", msg);
    return "Published Location message";
});

app.Run();