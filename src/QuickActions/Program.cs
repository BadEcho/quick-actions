using System.IO;
using BadEcho.Extensions;
using BadEcho.Extensibility.Extensions;
using BadEcho.Presentation.Extensions;
using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions;
using BadEcho.QuickActions.Options;
using BadEcho.QuickActions.Services;
using BadEcho.QuickActions.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

string userSettingsPath =
    Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                              builder.Environment.ApplicationName),
                 "usersettings.json");

builder.Configuration
       .AddExtensibility()
       .AddJsonFile(userSettingsPath, optional: true, reloadOnChange: true);

builder.Services
       .Configure<ScriptActionsOptions>(builder.Configuration.GetSection(ScriptActionsOptions.SectionName), userSettingsPath);

builder.Services
       .AddEventSourceLogForwarder()
       .AddApplication<App>();
    
builder.Services
       .AddSingleton<UserSettingsService>()
       .AddSingleton<Mediator>();

builder.Services
       .AddSingleton<MainWindow>()
       .AddSingleton<MainViewModel>()
       .AddSingleton<NavigationPaneViewModel>()
       .AddTransient<HomeViewModel>()
       .AddTransient<ActionsViewModel>();

var app = builder.Build();

app.Run();