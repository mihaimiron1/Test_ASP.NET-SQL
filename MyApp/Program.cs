using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ru-RU"),
    new CultureInfo("ro-RO") 
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ro-RO");  // default language
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "ro", "ru" };
    options.SetDefaultCulture(supportedCultures[0])
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});



// for database connection
builder.Services.AddDbContext<UniversitateaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseRequestLocalization();

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
