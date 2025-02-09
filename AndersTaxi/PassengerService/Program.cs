using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

var builder = WebApplication.CreateBuilder(args);

//inject
var factory = await CommunicationHandlerFactory.Initialize(GlobalConfig.HostName);
var consumerHandler = await factory.CreateConsumerHandler("passenger_queue");
var producerHandler = await factory.CreateProducerHandler("taxi.topic");

builder.Services.AddSingleton<CommunicationHandlerFactory>(cf => factory);
builder.Services.AddSingleton<ConsumerHandler>(ch => consumerHandler);
builder.Services.AddSingleton<ProducerHandler>(ph => producerHandler);

var app = builder.Build();

app.MapGet("/", () => "Passenger Says Hello!");


app.Run();