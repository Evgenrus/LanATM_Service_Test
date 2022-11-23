using MassTransit;
using Order.Consumers;
using Order.Database;
using Order.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderDbContext>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRabbitmqService, RabbitmqService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<FinishOrderReqConsumer>();
    
    x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c =>
    {
        c.Host("rabbitmq://localhost", settings =>
        {
            settings.Username("guest");
            settings.Password("guest");
        });
        c.ReceiveEndpoint("order-finish-queue1", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 3000));
            e.ConfigureConsumer<FinishOrderReqConsumer>(context);
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