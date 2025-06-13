using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");

// API Endpoints
app.MapGet("/api/countdown/start-date", () =>
{
    var startDate = new DateTime(2022, 3, 21, 0, 0, 0, DateTimeKind.Utc);
    return Results.Ok(new { startDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") });
});

app.MapGet("/api/countdown/current-time", () =>
{
    var currentTime = DateTime.UtcNow;
    return Results.Ok(new { currentTime = currentTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") });
});

app.MapGet("/api/countdown/calculate", () =>
{
    var startDate = new DateTime(2022, 3, 21, 0, 0, 0, DateTimeKind.Utc);
    var currentTime = DateTime.UtcNow;
    var timeSpan = currentTime - startDate;
    
    var years = (int)(timeSpan.TotalDays / 365.25);
    var months = (int)((timeSpan.TotalDays % 365.25) / 30.44);
    var days = (int)(timeSpan.TotalDays % 30.44);
    var hours = timeSpan.Hours;
    var minutes = timeSpan.Minutes;
    var seconds = timeSpan.Seconds;
    var milliseconds = timeSpan.Milliseconds;
    
    return Results.Ok(new
    {
        years,
        months,
        days,
        hours,
        minutes,
        seconds,
        milliseconds,
        totalDays = (int)timeSpan.TotalDays,
        totalHours = (int)timeSpan.TotalHours,
        totalMinutes = (int)timeSpan.TotalMinutes,
        totalSeconds = (int)timeSpan.TotalSeconds,
        totalMilliseconds = (long)timeSpan.TotalMilliseconds
    });
});

app.Run();