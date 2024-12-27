using DAL;
using DAL.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === DB CONFIG ===

var connectionString = $"Data Source={FileHelper.BasePath}app.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<AppDbContextFactory>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Game.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();


// builder.Services.AddScoped<IConfigRepository, ConfigRepositoryJson>();
// builder.Services.AddScoped<IGameRepository, GameRepositoryJson>();
// builder.Services.AddScoped<IGamesAndPlayersRepository, GamesAndPlayersJson>();
builder.Services.AddScoped<IConfigRepository, DbConfigRepository>();
builder.Services.AddScoped<IGameRepository, DbGameRepository>();

// =================

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseSession();

app.Run();