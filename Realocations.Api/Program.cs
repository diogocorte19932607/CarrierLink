using Microsoft.EntityFrameworkCore;
using VagasApp.Data;
using VagasApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para permitir chamadas do Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200") // porta do Angular
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Conexão com banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VagaContext>(options =>
    options.UseSqlServer(connectionString));

// Serviços customizados
builder.Services.AddScoped<VagaService>();

var app = builder.Build();

// Swagger em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ATENÇÃO: aplicar CORS antes de Authorization
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
