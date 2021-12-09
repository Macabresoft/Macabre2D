namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;

/// <summary>
/// A <see cref="IDataObject" /> that wraps an <see cref="object" />.
/// </summary>
public class GenericDataObject : IDataObject {
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericDataObject" /> class.
    /// </summary>
    public GenericDataObject(object genericObject, string name) {
        this.GenericObject = genericObject ?? throw new ArgumentNullException(nameof(genericObject));
        this._name = name;
    }

    /// <summary>
    /// Gets the generic object.
    /// </summary>
    public object GenericObject { get; }

    /// <inheritdoc />
    public bool Contains(string dataFormat) {
        return false;
    }

    /// <inheritdoc />
    public object Get(string dataFormat) {
        return this.GenericObject;
    }

    public IEnumerable<string> GetDataFormats() {
        return Enumerable.Empty<string>();
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFileNames() {
        return Enumerable.Empty<string>();
    }

    /// <inheritdoc />
    public string GetText() {
        return this._name;
    }
}