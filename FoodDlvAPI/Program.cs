using FoodDlvAPI.Hubs;
using FoodDlvAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//�[�J SignalR
builder.Services.AddSignalR();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//���U�n�ϥΪ� database ���O
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("FoodDelivery")));

var app = builder.Build();

// ���\ react app �s�� �� api �������� 
app.UseCors(options =>
options.WithOrigins("http://localhost:5129")
.AllowAnyMethod()
.AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//�[�J Hub
app.MapHub<OrderHub>("/OrderHub");


app.Run();
