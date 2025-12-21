// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

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
       .Configure<ScriptActionsOptions>(builder.Configuration.GetSection(ScriptActionsOptions.SectionName), userSettingsPath)
       .Configure<MappingOptions>(builder.Configuration.GetSection(MappingOptions.SectionName), userSettingsPath)
       .Configure<AppearanceOptions>(builder.Configuration.GetSection(AppearanceOptions.SectionName), userSettingsPath);

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
       .AddTransient<ActionsViewModel>()
       .AddTransient<MappingsViewModel>();

var app = builder.Build();

app.Run();