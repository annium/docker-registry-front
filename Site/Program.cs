using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Site;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>(nameof(App));
builder.Services.Register();
var app = builder.Build();
app.Services.Setup();
await app.RunAsync();