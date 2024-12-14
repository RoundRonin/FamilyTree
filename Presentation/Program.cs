using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.DAL;
using FamilyTreeBlazor.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

using FamilyTreeBlazor.presentation.Components;
using FamilyTreeBlazor.BLL.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from the shared method
var connectionString = DbContextConfigurationHelper.BuildConnectionString();

builder.Services.AddDbContext<ApplicationContext>(options =>
    DbContextConfigurationHelper.Configure((DbContextOptionsBuilder<ApplicationContext>)options, connectionString));

// Register repositories and services with DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<IFamilyTreeService, FamilyTreeService>();
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
