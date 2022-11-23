using Catalog.Consumers;
using Catalog.Database;
using Catalog.Models;
using Catalog.Services;
using MassTransit;
using Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CatalogDbContext>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IRabbitmqService, RabbitmqService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ItemModelListCheckConsumer>();
    x.AddConsumer<ItemModelCheckConsumer>();
    x.AddConsumer<ItemsOrderConsumer>();
    x.AddConsumer<RestockRequestConsumer>();

    x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c =>
    {
        c.Host("rabbitmq://localhost", settings =>
        {
            settings.Username("guest");
            settings.Password("guest");
        });
        c.ReceiveEndpoint("items-queue1", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 3000));
            e.ConfigureConsumer<ItemModelListCheckConsumer>(context);
            e.ConfigureConsumer<ItemModelCheckConsumer>(context);
        });
        c.ReceiveEndpoint("items-restock-queue1", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 3000));
            e.ConfigureConsumer<RestockRequestConsumer>(context);
        });
        c.ReceiveEndpoint("items-order-queue1", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 3000));
            e.ConfigureConsumer<ItemsOrderConsumer>(context);
        });
    }));
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