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

namespace BadEcho.QuickActions;

/// <summary>
/// Provides the Quick Actions application.
/// </summary>
internal sealed partial class App
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App(MainWindow window)
    {
        InitializeComponent();

        MainWindow = window;

        window.InitializeComponent();
        window.Show();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <remarks>
    /// This is required because WPF's MSBuild tasks will generate an application entry point that expects App
    /// to have a default constructor, even though our own application entry point in Program.cs is what will actually
    /// be called.
    /// </remarks>
    private App() 
        => InitializeComponent();
}
