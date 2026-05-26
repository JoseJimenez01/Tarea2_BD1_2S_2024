using Microsoft.EntityFrameworkCore;
using Tarea2_BD1.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Search .env
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

//Get connection
var conexion = Environment.GetEnvironmentVariable("ConnectionStrings__conexion");
if (string.IsNullOrWhiteSpace(conexion))
{
    throw new Exception("La cadena de conexión no fue encontrada.");
}

//Add connection to the context
builder.Services.AddDbContext<Dbtarea2Context>(opt => opt.UseSqlServer(conexion));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Login/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",

    //El siguiente es el oficial:
    pattern: "{controller=Login}/{action=SignIn}/{id?}");

app.Run();
