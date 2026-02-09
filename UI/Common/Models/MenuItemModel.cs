namespace Macabre2D.UI.Common;

using System.Windows.Input;
using Macabresoft.Core;

/// <summary>
/// A model for menu item generation.
/// </summary>
public sealed class MenuItemModel : PropertyChangedNotifier {
    private ICommand _command;
    private object _commandParameter;
    private string _header;
    private string _toolTip;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemModel" /> class.
    /// </summary>
    public MenuItemModel() : this(string.Empty, string.Empty, null, null) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemModel" /> class.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="command">The command.</param>
    /// <param name="commandParameter">The command parameter.</param>
    public MenuItemModel(string header, ICommand command, object commandParameter) : this(header, header, command, commandParameter) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemModel" /> class.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="toolTip">The tool tip.</param>
    /// <param name="command">The command.</param>
    /// <param name="commandParameter">The command parameter.</param>
    public MenuItemModel(string header, string toolTip, ICommand command, object commandParameter) {
        this.Header = header;
        this.ToolTip = toolTip;
        this.Command = command;
        this.CommandParameter = commandParameter;
    }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    public ICommand Command {
        get => this._command;
        set => this.Set(ref this._command, value);
    }

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    public object CommandParameter {
        get => this._commandParameter;
        set => this.Set(ref this._commandParameter, value);
    }

    /// <summary>
    /// Gets or sets the header.
    /// </summary>
    public string Header {
        get => this._header;
        set => this.Set(ref this._header, value);
    }

    /// <summary>
    /// Gets or sets the tool tip.
    /// </summary>
    public string ToolTip {
        get => this._toolTip;
        set => this.Set(ref this._toolTip, value);
    }
}