using Microsoft.EntityFrameworkCore;
using LawyerHelper.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfigurationRoot config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllers();
builder.Services.AddDbContext<LawyerHelperContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("Default") ?? Environment.GetEnvironmentVariable("Default") ?? ""));
    //.UseInMemoryDatabase("Default"));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Lawyer Helper", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lawyer Helper v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();