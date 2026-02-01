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

namespace BadEcho.QuickActions.Extensibility;

/// <summary>
/// Provides the result of executing an action.
/// </summary>
public sealed class ActionResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionResult"/> class.
    /// </summary>
    /// <param name="success">Value indicating if the action executed successfully.</param>
    /// <param name="error">An error message describing why the action failed.</param>
    private ActionResult(bool success, string error)
    {
        Success = success;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating if the action executed successfully.
    /// </summary>
    public bool Success
    { get; }

    /// <summary>
    /// Gets an error message describing why the action failed.
    /// </summary>
    public string Error
    { get; }

    /// <summary>
    /// Creates a result that indicates success.
    /// </summary>
    /// <returns>An <see cref="ActionResult"/> instance indicating success.</returns>
    public static ActionResult Ok() 
        => new(true, string.Empty);

    /// <summary>
    /// Creates a result that indicates failure.
    /// </summary>
    /// <param name="error">An error message describing why the action failed.</param>
    /// <returns>An <see cref="ActionResult"/> instance indicating failure.</returns>
    public static ActionResult Fail(string error)
        => new(false, error);
}

