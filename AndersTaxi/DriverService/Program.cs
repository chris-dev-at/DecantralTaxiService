using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

var builder = WebApplication.CreateBuilder(args);

//inject
var factory = await CommunicationHandlerFactory.Initialize(GlobalConfig.HostName);
var consumerHandler = await factory.CreateConsumerHandler("driver_queue");
var producerHandler = await factory.CreateProducerHandler("taxi.topic");

builder.Services.AddSingleton<CommunicationHandlerFactory>(cf => factory);
builder.Services.AddSingleton<ConsumerHandler>(ch => consumerHandler);
builder.Services.AddSingleton<ProducerHandler>(ph => producerHandler);

var app = builder.Build();

app.MapGet("/", () => "Driver Says Hello!");

//create endpoint for driver to update his location, all parameters over query string
app.MapGet("/updateLocation", async (string id, int x, int y, string state) =>
{
    var location = new Location()
    {
        X = x,
        Y = y
    };
    var driver = new Driver()
    {
        Id = id,
        Location = location,
        State = Enum.Parse<DriverState>(state)
    };
    var message = new LocationUpdateMessage()
    {
        Driver = driver
    };
    await producerHandler.SendMessageAsync("location", message);
    return "Published Location message";
});

app.Run();