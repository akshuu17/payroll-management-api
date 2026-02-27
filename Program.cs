using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ?? Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ?? Controllers
builder.Services.AddControllers();

// ?? Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ?? Email Service
builder.Services.AddScoped<EmailService>();

builder.Services.AddHostedService<EventReminderService>();

// ?? CORS (Allow All - For Flutter + Angular + Postman)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// ?? Swagger (Development Only Recommended)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ?? Enable CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();