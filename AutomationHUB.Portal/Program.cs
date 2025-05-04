using AutomationHUB.Portal.ElsaStudio.Extensions;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

builder.Services.AddElsaStudio(configuration);

//builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

//app.MapRazorComponents<R>()
//    .AddInteractiveServerRenderMode();

app.UseElsaStudio();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
