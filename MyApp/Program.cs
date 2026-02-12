using Microsoft.Extensions.FileProviders;
using MyApp.Repositories;
using MyApp.Services;

var builder = WebApplication.CreateBuilder(args);

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

// Inregistrare Connection Factory
builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
    new DbConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));

// Inregistrare Repositories
builder.Services.AddScoped<IAssignedVoterRepository, AssignedVoterRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();

// Memory Cache
builder.Services.AddMemoryCache();

// Background Service pentru actualizare automată
builder.Services.AddHostedService<StatisticsCacheService>();

// Don't stop the web host if a BackgroundService throws (we log and retry instead)
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles(); 

// Permite accesul la node_modules
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "node_modules")),
    RequestPath = "/node_modules"
});


app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Ruta dedicată pentru AssignedVoters pentru a evita orice conflict
app.MapControllerRoute(
    name: "assignedvoters",
    pattern: "AssignedVoters/{action=Index}/{id?}",
    defaults: new { controller = "AssignedVoters" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Rezultate}/{action=PrimariMunicipali}/{id?}")
    .WithStaticAssets();



app.Run();