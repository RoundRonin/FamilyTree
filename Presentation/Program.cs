using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.DAL;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL.Infrastructure;
using FamilyTreeBlazor.presentation.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

using FamilyTreeBlazor.presentation.Components;
using FamilyTreeBlazor.BLL.DTOs;

var builder = WebApplication.CreateBuilder(args);

// UI
builder.Services.AddMudServices();

var connectionString = DbContextConfigurationHelper.BuildConnectionString();

builder.Services.AddDbContext<ApplicationContext>(options =>
    DbContextConfigurationHelper.Configure((DbContextOptionsBuilder<ApplicationContext>)options, connectionString));

//builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionStrig));

// Register repositories and services with DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<IFamilyTreeService, FamilyTreeService>();
builder.Services.AddScoped<IRepository<Person>, Repository<Person>>();
builder.Services.AddScoped<IRepository<Relationship>, Repository<Relationship>>();
builder.Services.AddScoped<AppState>();

builder.Services.AddSingleton<TreeCacheDTO>();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
