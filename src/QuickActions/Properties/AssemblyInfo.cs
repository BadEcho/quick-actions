using System.Diagnostics.CodeAnalysis;
using System.Windows;

[assembly: ThemeInfo(ResourceDictionaryLocation.None,
                     ResourceDictionaryLocation.SourceAssembly)]

[assembly: SuppressMessage("Design",
                           "CA1062: Validate arguments of public methods",
                           Scope = "member",
                           Target = "M:BadEcho.QuickActions.App.#ctor(BadEcho.QuickActions.MainWindow)",
                           Justification = "This type is meant to be initialized by a dependency injection container. This code analysis rule seems to ignore interfaces of commonly injected types, but it gets flagged for custom service types. This is unnecessary, due to the fact that the runtime will throw an exception if any of the injected parameters cannot be obtained.")]