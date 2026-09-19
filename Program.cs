// Pengaturan ini memaksa .NET dan Npgsql menyelaraskan format DateTime lama/lokal menjadi kompatibel dengan pemformatan database
using MassTransit;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Parameters;
using Microsoft.EntityFrameworkCore;
using Purchase.Msv.Consumers;
using Purchase.Msv.Extensions;
using Purchase.Msv.Models;
using Purchase.Msv.Profiles;
using Purchase.Msv.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPurchaseTelemetry(builder.Configuration);

builder.Services.AddDbContext<PurchaseMsvDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PurchaseMsvDBConnection"))); 

builder.Services.AddAutoMapper(x=>{ },
typeof(MappingProfile).Assembly);   

builder.Services.AddControllers();

builder.Services.AddScoped<PurchaseService>();

builder.Services.AddCustomMassTransit(builder.Configuration);

var app = builder.Build();
app.UseRouting();
app.MapControllers();


app.Run();

