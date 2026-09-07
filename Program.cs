// Pengaturan ini memaksa .NET dan Npgsql menyelaraskan format DateTime lama/lokal menjadi kompatibel dengan pemformatan database
using Microsoft.EntityFrameworkCore;
using Purchase.Msv.Models;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PurchaseMsvDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PurchaseMsvDBConnection"))); 

builder.Services.AddControllers();

// Add services to the container.

var app = builder.Build();

app.MapControllers();


app.Run();

