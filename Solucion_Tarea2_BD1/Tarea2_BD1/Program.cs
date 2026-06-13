using Microsoft.EntityFrameworkCore;
using Tarea2_BD1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization();

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

//Add authentication with a cookie with time expiration of 2 hours
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/login";
        options.AccessDeniedPath = "/Login/Denied";

        options.Cookie.Name = "AuthCookie";

        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Login/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Change languages
var supportedCultures = new[]
{
    new CultureInfo("es"),
    new CultureInfo("en")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//Validate auth with the cookie
app.UseAuthentication();
//Review atributes like role, etc.
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",

    //El siguiente es el oficial:
    pattern: "{controller=Login}/{action=SignIn}/{id?}");

//Redirect to login
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();
