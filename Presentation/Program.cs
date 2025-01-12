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
using FamilyTreeBlazor.presentation.Controllers.Interfaces;
using FamilyTreeBlazor.presentation.Controllers;
using FamilyTreeBlazor.presentation.State.EditState;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State;
using FamilyTreeBlazor.presentation.State.ViewState.Interfaces;
using FamilyTreeBlazor.presentation.State.ViewState;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.State.AncestorAgeState.Interfaces;
using FamilyTreeBlazor.presentation.State.AncestorAgeState;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState.Interfaces;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState;

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

builder.Services.AddScoped<IAppStateContext, AppStateContext>();
builder.Services.AddScoped<IStateNotifier, StateNotifier>();
builder.Services.AddScoped<IViewToolState, ViewToolState>();
builder.Services.AddScoped<IEditToolState, EditToolState>();
builder.Services.AddScoped<IAncestorAgeToolState, AncestorAgeToolState>();
builder.Services.AddScoped<ICommonAncestorsToolState, CommonAncestorsToolState>();

builder.Services.AddScoped<IPresentationService, PresentationService>();
builder.Services.AddScoped<IAncestorService, AncestorService>();
builder.Services.AddScoped<IPersonRelationshipService, PersonRelationshipService>();
builder.Services.AddScoped<IRelationshipInfoService, RelationshipInfoService>();

builder.Services.AddScoped<ITreeController, TreeController>();
builder.Services.AddScoped<IPersonController, PersonController>();

builder.Services.AddSingleton<ITreeCache, TreeCacheDTO>();

// Register the component for JavaScript
builder.Services.AddServerSideBlazor(options =>
{
    options.RootComponents.RegisterForJavaScript<PersonCard>(identifier: "person-card");
    options.RootComponents.RegisterForJavaScript<InitCard>(identifier: "init-card");
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