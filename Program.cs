using InventoryGame.Data;
using InventoryGame.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<Db>();
builder.Services.AddSingleton<CurrencyService>();

var app = builder.Build();
app.UseDefaultFiles();   // index.html як стартова сторінка
app.UseStaticFiles();    // wwwroot
app.MapControllers();
app.Run();
