using Simulator.Components;
using Communications;
using Simulator.Components.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var communicationsFactory = await CommunicationHandlerFactory.Initialize(GlobalConfig.HostName);
var consumerHandler = await communicationsFactory.CreateConsumerHandler("simulator_queue");

builder.Services.AddScoped<HttpClient>();
builder.Services.AddSingleton<CommunicationHandlerFactory>(cf => communicationsFactory);
builder.Services.AddSingleton<ConsumerHandler>(ch => consumerHandler);
builder.Services.AddSingleton<EventStreamSimulator>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();