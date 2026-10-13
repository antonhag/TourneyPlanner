using Microsoft.EntityFrameworkCore;
using TourneyPlanner.API.Endpoints;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Application.Services;
using TourneyPlanner.Infrastructure.Data;
using TourneyPlanner.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TourneyPlannerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<ITournamentService, TournamentService>();

builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();

builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddScoped<IStandingService, StandingService>();

// Identity, endast användare med Organizer rollen får ändra data
builder.Services.AddAuthorization(options =>
    options.AddPolicy("Organizer", policy => policy.RequireRole("Organizer")));
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<TourneyPlannerDbContext>();

var app = builder.Build();

var organizerEmail = builder.Configuration["Organizer:Email"];
var organizerPassword = builder.Configuration["Organizer:Password"];

// seedar rollen och arrangörskontot vid varje start, men skapar bara det som saknas,
// uppgifterna kommer från user secrets, saknas de så hoppas seedningen över.
if (!string.IsNullOrEmpty(organizerEmail) && !string.IsNullOrEmpty(organizerPassword))
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Organizer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Organizer"));
    }
    
    var organizer = await userManager.FindByEmailAsync(organizerEmail);
    if (organizer == null)
    {
        organizer = new IdentityUser { UserName = organizerEmail, Email = organizerEmail };
        await userManager.CreateAsync(organizer, organizerPassword);
    }

    if (!await userManager.IsInRoleAsync(organizer, "Organizer"))
    {
        await userManager.AddToRoleAsync(organizer, "Organizer");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Identity
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/account").MapIdentityApi<IdentityUser>();

app.MapTournamentEndpoints();
app.MapMatchEndpoints();
app.MapParticipantEndpoints();
app.MapStandingsEndpoints();

app.Run();