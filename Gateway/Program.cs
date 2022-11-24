using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOcelot();
builder.Services.AddSwaggerGen();
builder.WebHost.ConfigureAppConfiguration(webBuilder =>
{
    webBuilder.AddJsonFile("ocelot.json");
});

builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseSwaggerForOcelotUI(options =>
{
    //options.DownstreamSwaggerEndPointBasePath = "/swagger";
    options.PathToSwaggerGenerator = "/swagger/docs";
    //options.SwaggerEndpoint("https://localhost:7000/swagger", "Gateway");
});

app.UseOcelot().Wait();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();