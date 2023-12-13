using FibonacciApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add Memory cache and Lazy cache
builder.Services.AddMemoryCache();
builder.Services.AddLazyCache();


builder.Services.AddMemoryCache(options =>
{
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
    options.SizeLimit = 100 * 1024 * 1024; // 1MB cache limit
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Middleware to handle exceptions

app.UseMiddleware<ExceptionMiddleware>();

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
