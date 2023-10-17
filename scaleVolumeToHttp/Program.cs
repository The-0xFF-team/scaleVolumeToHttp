using scaleVolumeToHttp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService(sp => sp.GetRequiredService<SerialReader>());
builder.Services.AddSingleton<SerialReader>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapGet("/",
        (SerialReader serialReader) =>
            Results.Ok(new { serialReader.GlassIsPresent, Consumption = serialReader.Consumed }))
    .WithName("GetSerialNumber")
    .WithOpenApi();

app.Run();