using AiPeopleFinder.Application;
using AiPeopleFinder.Components;
using AiPeopleFinder.Infrastructure;
using AiPeopleFinder.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));

// Add services to the container.
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplicationServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();