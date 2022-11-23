using Delivery.Consumers;
using Delivery.Database;
using Delivery.Models;
using Delivery.Service;
using Infrastructure.Models;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DeliveryDbContext>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IRabbitmqService, RabbtmqService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DeliveryRequestConsumer>();
    x.AddConsumer<DeliveryCancelConsumer>();
    
    x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c =>
    {
        c.Host("rabbitmq://localhost", settings =>
        {
            settings.Username("guest");
            settings.Password("guest");
        });
        c.ReceiveEndpoint("delivery-post-queue1", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 3000));
            e.ConfigureConsumer<DeliveryRequestConsumer>(context);
        });
        c.ReceiveEndpoint("delivery-cancel-queue1", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 3000));
            e.ConfigureConsumer<DeliveryCancelConsumer>(context);
        });
    }));
    
    x.AddRequestClient<IItemsToCheckList>(new Uri("rabbitmq://localhost/item-queue1"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();