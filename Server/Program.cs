using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Register();
builder.WebHost.UseKestrel(server =>
{
    server.AddServerHeader = false;
    server.ListenAnyIP(5002);
});

var app = builder.Build();

app.Services.Setup();
app.UseRouting();
app.UseCors(b => b
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .SetPreflightMaxAge(TimeSpan.FromDays(7))
);
app.MapControllers();

await app.RunAsync();