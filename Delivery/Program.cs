using Delivery.Database;
using Delivery.Models;
using Delivery.Service;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DeliveryDbContext>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddMassTransit(x =>
{
    x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c =>
    {
        c.Host("rabbitmq://localhost");
        c.ConfigureEndpoints(context);
    }));
    
    x.AddRequestClient<List<ItemModel>>();
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