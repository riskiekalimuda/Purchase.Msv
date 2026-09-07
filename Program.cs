// Pengaturan ini memaksa .NET dan Npgsql menyelaraskan format DateTime lama/lokal menjadi kompatibel dengan pemformatan database
using MassTransit;
using MessageMQCommon.Parameters;
using Microsoft.EntityFrameworkCore;
using Purchase.Msv.Models;
using Purchase.Msv.Profiles;
using Purchase.Msv.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PurchaseMsvDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PurchaseMsvDBConnection"))); 

builder.Services.AddAutoMapper(x=>{ },
typeof(MappingProfile).Assembly);   

builder.Services.AddControllers();

builder.Services.AddScoped<PurchaseService>();

var rabbitMQSetting = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMQParameter>()?? new RabbitMQParameter(); 

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<PurchaseMsvDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
        o.QueryDelay = TimeSpan.FromSeconds(10);
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMQSetting.Host, rabbitMQSetting.VirtualHost, h =>
        {
            h.Username(rabbitMQSetting.Username);
            h.Password(rabbitMQSetting.Password);
        });
    });
});

var app = builder.Build();

app.MapControllers();


app.Run();

