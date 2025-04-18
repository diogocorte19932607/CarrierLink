using Microsoft.EntityFrameworkCore;
using VagasApp.Data;
using VagasApp.Services;
using VagasApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


// ?? Connection String
var connectionString = "Server=DELL19932607\\SQLEXPRESS;Database=VagasDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<VagaContext>(options =>
    options.UseSqlServer(connectionString));

// Serviços customizados (opcional, ex: VagaService)
builder.Services.AddScoped<VagaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
