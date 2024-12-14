using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.DAL;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

using FamilyTreeBlazor.presentation.Services;
using FamilyTreeBlazor.presentation.Components;
using FamilyTreeBlazor.presentation.Components.Card;
using Microsoft.AspNetCore.Components.Web;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// UI
builder.Services.AddMudServices();

var connectionString = DbContextConfigurationHelper.BuildConnectionString();

builder.Services.AddDbContext<ApplicationContext>(options =>
    DbContextConfigurationHelper.Configure((DbContextOptionsBuilder<ApplicationContext>)options, connectionString), ServiceLifetime.Scoped);

builder.Services.AddScoped<DbContextFactory>();

builder.Services.AddSingleton<IServiceScopeFactory>(provider => provider.GetRequiredService<IServiceProvider>().CreateScope().ServiceProvider.GetRequiredService<IServiceScopeFactory>());
//builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionStrig));

// Register repositories and services with DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<IFamilyTreeService, FamilyTreeService>();
builder.Services.AddScoped<IRepository<Person>, Repository<Person>>();
builder.Services.AddScoped<IRepository<Relationship>, Repository<Relationship>>();

builder.Services.AddScoped<IStateNotifier, StateNotifier>(); 
builder.Services.AddScoped<IViewToolState, ViewToolState>();
builder.Services.AddScoped<IEditToolState, EditToolState>();
builder.Services.AddScoped<IAncestorAgeToolState, AncestorAgeToolState>();
builder.Services.AddScoped<ICommonAncestorsToolState, CommonAncestorsToolState>();

builder.Services.AddScoped<IAppStateService, AppStateService>();
builder.Services.AddScoped<ITreeService, TreeService>();

builder.Services.AddSingleton<ITreeCache, TreeCacheDTO>();

// Register the component for JavaScript
builder.Services.AddServerSideBlazor(options =>
{
    options.RootComponents.RegisterForJavaScript<PersonCard>(identifier: "person-card");
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

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