using TourneyPlanner.UI.Components;
using TourneyPlanner.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiUrl = builder.Configuration["ApiUrl"] ?? throw new InvalidOperationException("ApiUrl finns inte i config");

builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped(sp =>
{
    var handler = new AuthHeaderHandler(sp.GetRequiredService<AuthState>())
    {
        InnerHandler = new HttpClientHandler()
    };
    return new HttpClient(handler) { BaseAddress = new Uri(apiUrl) };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();