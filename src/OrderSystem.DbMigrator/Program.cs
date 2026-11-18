using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderSystem.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPersistence(builder.Configuration);

using var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();

var db = scope.ServiceProvider.GetRequiredService<OrderSystemDbContext>();
await db.Database.MigrateAsync();