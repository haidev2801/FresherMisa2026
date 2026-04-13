using FresherMisa2026.WebAPI.Middlewares;
using FresherMisa2026.Infrastructure;
using FresherMisa2026.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI
// Đăng ký các dependency injection cho layer Application và Infrastructure thông qua các extension method.
builder.Services.AddApplicationDI();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Trong môi trường dev bật swagger để test API nhanh
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Middlewares
// Global exception handler: bắt mọi exception không xử lý và trả ServiceResponse chuẩn
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
