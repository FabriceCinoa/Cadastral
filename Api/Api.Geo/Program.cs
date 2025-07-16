using Common.Library.Logger;
using Common.Repository.DBContext;
using GeoRepository.Repositories;
using NetTopologySuite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGeoRepository, GeoPostGreDBRepository>();
// Configuration NetTopologySuite
var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
builder.Services.AddSingleton(geometryFactory);
builder.Services.AddSingleton <ILogger>(new ConsoleLogger());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
}
app.UseSwaggerUI();


app.Run();
