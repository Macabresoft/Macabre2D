namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="KeyboardIconSet" />.
/// </summary>
[DataContract]
[Category("Keyboard Icon Sets")]
public class KeyboardIconSetCollection : NameableCollection<KeyboardIconSet> {
    /// <inheritdoc />
    public override string Name => "Keyboard Icon Sets";
}