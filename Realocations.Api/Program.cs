using Microsoft.EntityFrameworkCore;
using VagasApp.Data;
using VagasApp.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ?? Connection String

//var connectionString = "Server=DELL19932607\\SQLEXPRESS;Database=VagasDB;Trusted_Connection=True;TrustServerCertificate=True;";
//builder.Services.AddDbContext<VagaContext>(options =>
//    options.UseSqlServer(connectionString));


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VagaContext>(options =>
    options.UseSqlServer(connectionString));


//var connectionString = "Server=DELL19932607\\SQLEXPRESS;Database=VagasDB;Trusted_Connection=True;TrustServerCertificate=True;";
//builder.Services.AddDbContext<VagaContext>(options =>
//    options.UseSqlServer(connectionString));

// Serviços customizados (opcional, ex: VagaService)
builder.Services.AddScoped<VagaService>();

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
